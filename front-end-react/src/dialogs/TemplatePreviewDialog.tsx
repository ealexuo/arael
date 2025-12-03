import React, { useCallback, useEffect } from 'react'

// General Dependencies
import {
  DialogContent,
  DialogTitle,
  DialogContentText,
  Box,
  DialogActions,
  Button,
  Typography,
  Paper,
  Grid,
  TextField,
  FormGroup,
  FormControlLabel,
  Checkbox,
  Accordion,
  AccordionSummary,
  AccordionDetails,
  InputAdornment,
  Autocomplete,
  Skeleton,
  Stack,
} from "@mui/material";

import ExpandMoreIcon from '@mui/icons-material/ExpandMore';
import { useTranslation } from 'react-i18next';

// Toastr Dependencies
import { Campo, Plantilla, ValorLista } from '../types/Plantilla';
import { Proceso } from '../types/Proceso';

// Date Picker Dependencies
import { AdapterMoment } from '@mui/x-date-pickers/AdapterMoment';
import { LocalizationProvider } from '@mui/x-date-pickers/LocalizationProvider';
import { DatePicker } from '@mui/x-date-pickers/DatePicker';

// Other Dependencies
import { FieldType } from '../enums/FieldTypes';
import { enqueueSnackbar } from 'notistack';
import { templateService } from '../services/settings/templateService';

// Dialog parameters Type
type DialogProps = {
    mode: 'add'|'edit',
    selectedTemplate: Plantilla | undefined,
    selectedWorkflow: Proceso,
    onClose: (refreshWorkflowTemplatesList: boolean) => void
}

export default function TemplateAddEditDialog({mode, selectedTemplate, selectedWorkflow, onClose}: DialogProps) {

    // Local constants or varialbes    
    const [t] = useTranslation();
    
    const [loading, setLoading] = React.useState<boolean>(false);
    const [listValues, setlistValues] = React.useState<ValorLista[]>([]);

    const getFieldLabel = (field: Campo) => {
      return field.obligatorio ? `* ${field.nombre}` : field.nombre;
    }

    const getListValues = (field: Campo) => {

      let selectedParentValueId: number = 0;

      if(field.idCampoPadre !== 0) {
        const selectedParentValue = getListSelectedValue({...field, idCampo: field.idCampoPadre});
        if(selectedParentValue){
          selectedParentValueId = selectedParentValue.idValor;
        }
      }

      return listValues
          .filter((value) => 
              value.idSeccion === field.idSeccion && 
              value.idCampo === field.idCampo && 
              (
                value.idCampoPadre === 0 || 
                (value.idCampoPadre === field.idCampoPadre && value.idValorPadre === selectedParentValueId)
              )
          );
    }

    const getListSelectedValue = (field: Campo) => {
      return listValues.find(
          (value) => value.idSeccion === field.idSeccion && value.idCampo === field.idCampo && value.seleccionado
        ) || null;
    }

    const handleListValueChange = (field: Campo, newValue: any) => {

      const listValuesTemp = [...listValues];
      
      listValuesTemp.forEach((value) => {
        if (value.idSeccion === field.idSeccion && value.idCampo === field.idCampo) {
          value.seleccionado = false;
        }          
      });

      if (newValue) {

        const existingValueIndex = listValuesTemp.findIndex(
          (value) => value.idSeccion === field.idSeccion && value.idCampo === field.idCampo && value.idValor === newValue.idValor
        );
        
        if (existingValueIndex !== -1) {
          listValuesTemp[existingValueIndex].seleccionado = true;
        } 
        
      }
      
      setlistValues(listValuesTemp);
    }

    const renderField = (field: any) => {
        switch (field.idTipoCampo) {
            case FieldType.Text:
                return (
                  <TextField
                    fullWidth
                    type={'text'}
                    label={getFieldLabel(field)}
                    variant="standard"
                    inputProps={{ maxLength: field.longitud }}
                  />
                );
            case FieldType.TextArea:
                return (
                  <TextField
                    fullWidth
                    type={'text'}
                    label={getFieldLabel(field)}
                    variant="standard"
                    inputProps={{ maxLength: field.longitud }}
                    multiline
                  />
                );
            case FieldType.Number:
                return (
                  <TextField
                    fullWidth
                    type={'number'}
                    label={getFieldLabel(field)}
                    variant="standard"
                  />
                );
            case FieldType.Date:
                return (
                    <LocalizationProvider dateAdapter={AdapterMoment}>
                      <DatePicker
                        views={['year', 'month', 'day']}
                        label={getFieldLabel(field)}
                        slotProps={{ textField: { variant: "standard", fullWidth: true } }}
                      />
                  </LocalizationProvider>
                );
            case FieldType.Checkbox:
                return (
                    <FormGroup sx={{ display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
                      <FormControlLabel
                        control={<Checkbox />}
                        label={getFieldLabel(field)}
                      />
                    </FormGroup>
                );
            case FieldType.List: 
                return (
                    <Autocomplete                      
                      disablePortal
                      options={getListValues(field)}
                      getOptionLabel={(option) => option.nombre}
                      value={getListSelectedValue(field) || null}
                      fullWidth
                      renderInput={
                        (params) => <TextField {...params} label={getFieldLabel(field)} variant='standard' />
                      }
                      renderOption={(props, option) => (
                          <li {...props} key={option.idValor}>                                                        
                              {option.nombre}
                          </li>
                      )}
                      onChange={(event, newValue) => {
                          handleListValueChange(field, newValue);
                      }}
                    />
                );
            case FieldType.Currency:
                return (
                    <TextField
                        fullWidth
                        type="number"
                        label={getFieldLabel(field)}
                        variant="standard"
                        value={field.value}
                        InputProps={{
                            startAdornment: <InputAdornment position="start">Q</InputAdornment>,
                        }}
                    />
                );                
            default:
                return null;
        }
    }    

    // Fetch list values
    const fetchListValues = useCallback(async () => {

      setLoading(true);

      try {  
  
        const listValuesTemp: ValorLista[] = [];
        
        for (const section of selectedTemplate?.listaSecciones || []) {
          for (const field of section.listaCampos || []) {
            if (field.idTipoCampo === FieldType.List) {

              const response = await templateService.getListValues(
                field.idProceso, 
                field.idPlantilla, 
                field.idSeccion, 
                field.idCampo, 
                0, 
                0
              );
  
              if (response.statusText === "OK") {
                
                listValuesTemp.push(...response.data);

              } else {
                enqueueSnackbar("Ocurrió un error al obtener los valores de la lista.", {
                  variant: "error",
                });
              }
            }
          }
        }

        setlistValues(listValuesTemp);

      } catch (error: any) {
        
        enqueueSnackbar(          
          "Ocurrió un error al obtener los valores de la lista. Detalles: " +
          error.message,
          { variant: "error" }
        );
       
      } finally {
        setLoading(false);
      }
      
    }, [selectedTemplate]);
  
  
    useEffect(() => {
  
      fetchListValues();
  
    }, [fetchListValues]);

    return (
      <>
        <DialogTitle>{selectedTemplate?.nombre}</DialogTitle>
        <DialogContent>
          <DialogContentText>
            Previsualización de la Plantilla
          </DialogContentText>
          <Box sx={{ my: 3 }}>
            <Typography variant="subtitle1"></Typography>
            <Paper
              variant="outlined"
              sx={{ my: { xs: 2, md: 2 }, p: { xs: 2, md: 3 }, backgroundColor: 'rgba(246, 247, 248, 0.5)' }}
            >
              {
                loading ? (
                   <Stack spacing={1}>
                    <Skeleton variant="rectangular" width={610} height={60} />
                    <Skeleton variant="rectangular" width={610} height={60} />
                    <Skeleton variant="rectangular" width={610} height={60} />
                  </Stack>
                ) :
                (
                  <Box>
                    {selectedTemplate?.listaSecciones?.map((section) => (
                        <Accordion key={section.idSeccion} defaultExpanded = {false}>
                            <AccordionSummary
                              expandIcon={<ExpandMoreIcon />}                          
                            >
                              <Box display="flex" flexDirection="column">
                              <Typography component="span">{section.nombre}</Typography>
                              </Box>
                            </AccordionSummary>
                          <Box sx={{ px: 2 }}>
                            <Box
                              sx={{
                                borderBottom: 1,
                                borderColor: 'divider',
                                mb: 1,
                              }}
                            />
                          </Box>
                          <AccordionDetails>
                            <Grid container spacing={2}>
                            {
                              section?.listaCampos?.filter((field) => field.activo ).map((field, index) => (
                              <Grid item xs={12} sm={field.noColumnas} key={index} sx={{ mb: 2, mt: 2 }}>
                                {renderField(field)}
                              </Grid>
                              ))
                            }
                            </Grid>
                          </AccordionDetails>
                        </Accordion>
                    ))}
                  </Box>
                )
              }              
            </Paper>
          </Box>
        </DialogContent>
        <DialogActions>
          <Button
            variant="outlined"
            onClick={() => {
              onClose(false);
            }}
          >
            Cerrar
          </Button>          
        </DialogActions>
      </>
    );
}
