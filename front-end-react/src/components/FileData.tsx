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
import { Campo, ValorLista } from '../types/Plantilla';
import { Proceso } from '../types/Proceso';

// Date Picker Dependencies
import { AdapterMoment } from '@mui/x-date-pickers/AdapterMoment';
import { LocalizationProvider } from '@mui/x-date-pickers/LocalizationProvider';
import { DatePicker } from '@mui/x-date-pickers/DatePicker';

// Other Dependencies
import { FieldType } from '../enums/FieldTypes';
import { enqueueSnackbar } from 'notistack';
import { templateService } from '../services/settings/templateService';
import { Expediente } from '../types/Expediente';
import { fileService } from '../services/files/fileService';
import { ExpedienteCampoDatos, ExpedienteSeccionDatos } from '../types/ExpedienteDatos';

import { Controller, useForm } from 'react-hook-form';

type Props = {
    workflow: Proceso | null,  
    file: Expediente | null
}

export default function FileData({workflow, file}: Props) {

  // Local constants or varialbes    
  const [t] = useTranslation();
  
  const [loading, setLoading] = React.useState<boolean>(false);
  const [fileSectionsList, setFileSectionsList] = React.useState<ExpedienteSeccionDatos[]>([]);
  const [listValues, setlistValues] = React.useState<ValorLista[]>([]);

  const {
    register,
    handleSubmit,
    reset,
    watch,
    control,
    formState: { errors },
  } = useForm();
    
  const getFieldLabel = (field: Campo) => {
    return field.obligatorio ? `* ${field.nombre}` : field.nombre;
  }

  const getListValues = (section: ExpedienteSeccionDatos, field: ExpedienteCampoDatos) => {

    let selectedParentValueId: number = 0;

    if(field.idCampoPadre !== 0) {
      const selectedParentValue = getListSelectedValue(section, field);
      if(selectedParentValue){
        selectedParentValueId = selectedParentValue.idValor;
      }
    }

    return listValues
        .filter((value) => 
            value.idSeccion === section.idSeccion && 
            value.idCampo === field.idCampo && 
            (
              value.idCampoPadre === 0 || 
              (value.idCampoPadre === field.idCampoPadre && value.idValorPadre === selectedParentValueId)
            )
        );
  }

  const getListSelectedValue = (section: ExpedienteSeccionDatos, field: ExpedienteCampoDatos) => {

    let newvalue = listValues.find(
        (value) => value.idSeccion === section.idSeccion && value.idCampo === field.idCampo && value.seleccionado
      ) || null;

    return newvalue;
  }

  const handleListValueChange = (section: ExpedienteSeccionDatos, field: ExpedienteCampoDatos, newValue: ValorLista | null) => {

    const listValuesTemp = [...listValues];
    
    listValuesTemp.forEach((value) => {
      if (value.idSeccion === section.idSeccion && value.idCampo === field.idCampo) {
        value.seleccionado = false;
      }          
    });

    if (newValue) {

      const existingValueIndex = listValuesTemp.findIndex(
        (value) => value.idSeccion === section.idSeccion && value.idCampo === field.idCampo && value.idValor === newValue.idValor
      );
      
      if (existingValueIndex !== -1) {
        listValuesTemp[existingValueIndex].seleccionado = true;
      } 
      
    }
    
    setlistValues(listValuesTemp);
  }    

  const renderField = (section: any, field: any) => {

      const validationRules: Record<string, any> = {};
      const fieldIdentifier = `${section.idSeccion}-${field.idCampo}`;

      if (field.obligatorio) { 
        validationRules.required = `${field.nombre} es requerido.`; 
      }

      // if (field.pattern) {
      //   validationRules.pattern = { value: field.pattern, message: 'Invalid format' };
      // }
        
      if (field.longitud > 0) {
        validationRules.maxLength = { value: field.longitud, message: `Max length is ${field.longitud}` };
      }        

      switch (field.idTipoCampo) {
          case FieldType.Text:
              return (
                <TextField
                  fullWidth
                  type={'text'}
                  label={getFieldLabel(field)}
                  variant="standard"
                  inputProps={{ maxLength: field.longitud }}
                  defaultValue={field.valor}
                  {...register(fieldIdentifier, validationRules)}
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
                  defaultValue={field.valor}
                  {...register(fieldIdentifier, validationRules)}
                />
              );
          case FieldType.Number:
              return (
                <TextField
                  fullWidth
                  type={'number'}
                  label={getFieldLabel(field)}
                  variant="standard"
                  defaultValue={field.valor}
                  {...register(fieldIdentifier, { maxLength: 100 })} // TODO : Add proper validation
                  error={!!errors[fieldIdentifier]}
                  helperText={errors[fieldIdentifier]?.message?.toString()}
                />
              );
          case FieldType.Date:
              return (
                <LocalizationProvider dateAdapter={AdapterMoment}>
                    <Controller
                      name={fieldIdentifier}
                      control={control}
                      render={({ field: controllerField, fieldState }) => (
                        <DatePicker
                          views={['year', 'month', 'day']}
                          label={getFieldLabel(field)}
                          {...controllerField}
                          value={field.valor || null}
                          onChange={(date) => controllerField.onChange(date)}
                          slotProps={{
                            textField: {                                
                              fullWidth: true,
                              variant: "standard",
                              error: !!fieldState.error,
                              helperText: fieldState.error?.message || "",
                            },
                          }}
                        />
                      )}
                    />
                </LocalizationProvider>
              );
          case FieldType.Checkbox:
              return (
                  <FormGroup sx={{ display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
                    <FormControlLabel
                      control={<Checkbox />}
                      label={getFieldLabel(field)}
                      defaultChecked={field.valor === 'true' || field.valor === true}
                      {...register(fieldIdentifier)}
                    />
                  </FormGroup>
              );
          case FieldType.List: 
              return (
                  <Autocomplete                      
                    disablePortal
                    options={getListValues(section, field)}
                    getOptionLabel={(option) => option.nombre}
                    value={getListSelectedValue(section, field) || null}
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
                        handleListValueChange(section, field, newValue);
                    }}
                  />
              );
          case FieldType.Currency:
              return (
                  <TextField                        
                      fullWidth
                      type={'number'}
                      label={getFieldLabel(field)}
                      variant="standard"
                      defaultValue={field.valor}
                      InputProps={{
                          startAdornment: <InputAdornment position="start">Q</InputAdornment>,
                      }}
                      {...register(fieldIdentifier, { maxLength: 100 })} // TODO : Add proper validation
                      error={!!errors[fieldIdentifier]}
                      helperText={errors[fieldIdentifier]?.message?.toString()}
                  />
              );                
          default:
              return null;
      }
  }    

  // Fetch list values
  const fetchListValues = useCallback(async () => {

    if (!workflow || !file) return;

    setLoading(true);

    try {  

      const listValuesTemp: ValorLista[] = [];
      
      for (const section of fileSectionsList || []) {
        for (const field of section.listaCampos || []) {
          if (field.idTipoCampo === FieldType.List) {

            const response = await templateService.getCurrentTemplateListValues(
              workflow.idProceso, 
              section.idPlantilla, 
              section.idSeccion, 
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
    
  }, [file, workflow, fileSectionsList]); 
  
  // Fetch file data
  const fetchFileData = useCallback(async () => { 

    if (!file) return;

    setLoading(true);

    try {          
      
      const fileDataResponse = await fileService.getData(file.idExpediente);
      
      if (fileDataResponse.statusText === "OK") {
        
        setFileSectionsList(fileDataResponse.data);          

      } else {
        enqueueSnackbar("Ocurrió un error al obtener los datos del expediente.", {
          variant: "error",
        });
      }

    } catch (error: any) {
      
      enqueueSnackbar(          
        "Ocurrió un error al obtener los datos del expediente. Detalles: " +
        error.message,
        { variant: "error" }
      );
      
    } finally {
      setLoading(false);
    }

  }, [file]);   
  
  useEffect(() => {  
    fetchFileData();
  }, [fetchFileData]);

  useEffect(() => {  
    fetchListValues();  
  }, [fetchListValues]);    

  const onSubmit = (data: any) => {

    console.log(data);

    let tempFileSectionList = [...fileSectionsList];

    tempFileSectionList.forEach((section: ExpedienteSeccionDatos) => {    
      section.listaCampos.forEach((field: ExpedienteCampoDatos) => {
        const fieldIdentifier = `${section.idSeccion}-${field.idCampo}`;
        field.valor = data[fieldIdentifier];
      });
    });

    console.log(tempFileSectionList);

    reset();
  }
    
  return (
    <Box sx={{ my: 3 }}
      component="form"
      onSubmit={handleSubmit(onSubmit)}
    >
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
              {fileSectionsList?.map((section) => (
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
                          {renderField(section, field)}
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
      <Button
        variant="contained"
        disableElevation
        type="submit"          
      >
        Guardar
      </Button>         
    </Box>

  );
}