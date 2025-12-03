import React, { useCallback, useEffect, useState } from 'react'

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
  Autocomplete,
  TableContainer,
  Table,
  TableHead,
  TableRow,
  TableCell,
  TableBody,
  IconButton,
  Tooltip,
  Fab,
  Dialog,
  Checkbox,
} from "@mui/material";

import ArrowUpwardIcon from '@mui/icons-material/ArrowUpward'
import ArrowDownwardIcon from '@mui/icons-material/ArrowDownward'

// React Form Dependencies
import { useTranslation } from 'react-i18next';

// Toastr Dependencies
import { useSnackbar } from 'notistack';
import DeleteIcon from '@mui/icons-material/Delete';
import AddIcon from '@mui/icons-material/Add';
import AlertDialog from '../components/AlertDialog';
import { Campo, ValorLista } from '../types/Plantilla';
import { templateService } from '../services/settings/templateService';

// Dialog parameters Type
type DialogProps = {
    selectedField: Campo | undefined,
    onClose: (refreshSectionsList: boolean) => void
}

// Other global variables

export default function FieldListValuesDialog({selectedField, onClose}: DialogProps) {
    // Local constants or varialbes
    const [t] = useTranslation();
    const { enqueueSnackbar } = useSnackbar();
    const [loading, setLoading] = useState<boolean>(false);
    const [newValue, setNewValue] = useState<string>('');
    const [listValuesList, setListValuesList] = useState<ValorLista[]>([]);
    const [selectedValue, setSelectedValue] = useState<ValorLista | null>(null);
    const [openDeleteValueDialog, setOpenDeleteValueDialog] = useState<boolean>(false);
    const [openDeleteAllValuesDialog, setOpenDeleteAllValuesDialog] = useState<boolean>(false);
    const [parentListValuesList, setParentListValuesList] = useState<ValorLista[]>([]);
    const [selectedParentValue, setSelectedParentValue] = useState<ValorLista | null>(null);
    
    const handleAddValueToList = async () => {
        
        setLoading(true);

        const maxOrder = listValuesList.length > 0 ? Math.max(...listValuesList.map(filedValue => filedValue.orden)) : 0

        try{
            if (selectedField) {
                const response = await templateService.addListValue({
                    idEntidad: selectedField ? selectedField.idEntidad : 0,
                    idProceso: selectedField ? selectedField.idProceso : 0,
                    idPlantilla: selectedField ? selectedField.idPlantilla : 0,
                    idSeccion: selectedField ? selectedField.idSeccion : 0,
                    idCampo: selectedField ? selectedField.idCampo : 0,
                    idValor: 0,
                    orden: maxOrder + 1,
                    idCampoPadre: selectedField.idCampoPadre,
                    idValorPadre: selectedParentValue ? selectedParentValue.idValor : 0,
                    nombre: newValue,
                    predeterminado: false
                });
    
                if(response.statusText === 'OK') {
                    fetchListValues();
                    setNewValue('');
                    enqueueSnackbar('Valor agregado a la lista correctamente.', { variant: 'success' });
                }
                else {
                    enqueueSnackbar('Ocurrió un error al guardar el valor de la lista.', { variant: 'error' });
                }            
            }
        }
        catch(error: any){
            enqueueSnackbar('Ocurrió un error al guardar el valor de la lista. Detalles: ' + error.message, { variant: 'error' });    
        }
        finally {            
            setNewValue('');
            setLoading(false);    
        }       
        
    };
    
    const fetchListValues = useCallback(async () => {
        setLoading(true);

        try {
            const response = await templateService.getListValues(
                selectedField ? selectedField.idProceso : 0, 
                selectedField ? selectedField.idPlantilla : 0, 
                selectedField ? selectedField.idSeccion : 0, 
                selectedField ? selectedField.idCampo : 0, 
                selectedField ? selectedField.idCampoPadre : 0, 
                selectedParentValue ? selectedParentValue.idValor : 0
            );
            if(response.statusText === 'OK') {
                setListValuesList(response.data);
            }
            else {
                enqueueSnackbar('Ocurrió un error al obtener los valores de la lista.', { variant: 'error' });
            }        
        }
        catch(error: any){
            enqueueSnackbar('Ocurrió un error al obtener los valores de la lista. Detalles: ' + error.message, { variant: 'error' });
        }
        finally {
            setLoading(false);
        }

    }, [enqueueSnackbar, selectedField, selectedParentValue]);

    const fetchParentListValues = useCallback(async () => {
        
        if(!selectedField || !selectedField.idCampoPadre || selectedField.idCampoPadre <= 0) {
            return;
        }
        
        setLoading(true);

        try {
            const response = await templateService.getListValues(
                selectedField ? selectedField.idProceso : 0, 
                selectedField ? selectedField.idPlantilla : 0, 
                selectedField ? selectedField.idSeccion : 0, 
                selectedField ? selectedField.idCampoPadre : 0, 
                0, 
                0
            );
            if(response.statusText === 'OK') {
                setParentListValuesList(response.data);
            }
            else {
                enqueueSnackbar('Ocurrió un error al obtener los valores de la lista.', { variant: 'error' });
            }        
        }
        catch(error: any){
            enqueueSnackbar('Ocurrió un error al obtener los valores de la lista. Detalles: ' + error.message, { variant: 'error' });
        }
        finally {
            setLoading(false);
        }

    }, [enqueueSnackbar, selectedField]);

    // Delete value dialog

    const handleOpenDeleteValueDialog = (value: ValorLista) => {
        setSelectedValue(value);
        setOpenDeleteValueDialog(true);
    };

    const handleCloseDeleteValueDialog = () => {
        setOpenDeleteValueDialog(true);
    };

    const handleCloseDeleteValueDialogFromAction = async (actionResult: boolean) => {
        
        if(actionResult) {
            setLoading(true);

            try{
                if (selectedValue) {
                    const response = await templateService.deleteListValue(
                        selectedValue.idProceso, 
                        selectedValue.idPlantilla, 
                        selectedValue.idSeccion, 
                        selectedValue.idCampo,
                        selectedValue.idValor
                    );        
                    
                    if(response.statusText === 'OK') {
                        fetchListValues();
                        enqueueSnackbar('Valor eliminado de la lista correctamente.', { variant: 'success' });
                    }                    
                    else {
                        enqueueSnackbar('Ocurrió un error al eliminar el valor de la lista.', { variant: 'error' });   
                    }            
                }
            }
            catch(error: any){
                if(error.response.data === 'VALOR_PADRE') {
                        enqueueSnackbar('No se pueden eliminar el valor, una o más listas dependen de él.', { variant: 'error' });
                }
                else {
                    enqueueSnackbar('Ocurrió un error al eliminar el valor de la lista.', { variant: 'error' });
                }
            }
            finally {            
                setLoading(false);    
            }  
        }       

        setOpenDeleteValueDialog(false);
    };


    // Delete all values dialog
    const handleOpenDeleteAllValuesDialog = () => {
        setOpenDeleteAllValuesDialog(true);
    };

    const handleCloseDeleteAllValuesDialog = () => {
        setOpenDeleteAllValuesDialog(true);
    };

    const handleCloseDeleteAllValuesDialogFromAction = async (actionResult: boolean) => {
        
        if(actionResult) {
            setLoading(true);

            try{
                if (selectedField) {
                    const response = await templateService.deleteAllListValues(
                        selectedField.idProceso, 
                        selectedField.idPlantilla, 
                        selectedField.idSeccion, 
                        selectedField.idCampo
                    );        
                    
                    if(response.statusText === 'OK') {
                        fetchListValues();
                        enqueueSnackbar('Valores de lista eliminados correctamente.', { variant: 'success' });
                    }                    
                    else {
                        enqueueSnackbar('Ocurrió un error al eliminar los valores de la lista.', { variant: 'error' });   
                    }            
                }
            }
            catch(error: any){
                if(error.response.data === 'CAMPO_PADRE') {
                        enqueueSnackbar('No se pueden eliminar los valores de una lista predecesora.', { variant: 'error' });
                }
                else {
                    enqueueSnackbar('Ocurrió un error al eliminar los valores de la lista.', { variant: 'error' });
                }
            }
            finally {            
                setLoading(false);    
            }  
        }       

        setOpenDeleteAllValuesDialog(false);
    };

    const handleDefaultValue = async (value: ValorLista, isDefault: boolean) => {
        
        setLoading(true);

        try{
            value.predeterminado = isDefault;
            const response = await templateService.setDefaultValue(value);

            if(response.statusText === 'OK') {
                fetchListValues();
                enqueueSnackbar('Valor de lista actualizado correctamente.', { variant: 'success' });
            }
            else {
                enqueueSnackbar('Ocurrió un error al guardar el valor de la lista.', { variant: 'error' });
            }
        }
        catch(error: any){
            enqueueSnackbar('Ocurrió un error al guardar el valor de la lista. Detalles: ' + error.message, { variant: 'error' });    
        }
        finally {            
            setLoading(false);    
        }   
    };

    // Parent field methods
    
    const handleParentListValuesChange = (newParentValue: ValorLista | null) => {
        setSelectedParentValue(newParentValue);        
    };

    const handleMoveValue = async (value: ValorLista, direction: string) => {

        if(listValuesList) {
        
            const valuesListTemp = [...listValuesList];
            const index1 = valuesListTemp.findIndex((v) => v.idValor === value.idValor);
            const index2 = direction === 'up' ? index1 - 1 : index1 + 1;      
    
            [valuesListTemp[index1], valuesListTemp[index2]] = [valuesListTemp[index2], valuesListTemp[index1]];      
    
            for (let i = 0; i < valuesListTemp.length; i++) {
                valuesListTemp[i].orden = i + 1;
            }
    
            try {
                await templateService.changeListValuesOrder(valuesListTemp);
                enqueueSnackbar("Orden de valores actualizado.", { variant: "success" });
                fetchListValues();            
            }
            catch (error) {
                enqueueSnackbar("Ocurrió un error al actualizar el orden de los valores.", { variant: "error" });
            }      
        }
        
    }

    useEffect (() => {        
        
        if(selectedField && selectedField.idCampoPadre && selectedField.idCampoPadre > 0 && !selectedParentValue) {
            fetchParentListValues();
        }

        fetchListValues();

    }, [fetchListValues, fetchParentListValues, selectedParentValue, selectedField]);

    
    return (
        <>
            <DialogTitle>{"Valores de la lista " + selectedField?.nombre }</DialogTitle>
            <DialogContent>
            <DialogContentText>Ingrese los valores pertenecientes a la lista seleccionada.</DialogContentText>
            <Box sx={{ my: 3 }}>
                <Typography variant="subtitle1"></Typography>
                <Paper
                variant="outlined"
                sx={{ my: { xs: 2, md: 2 }, p: { xs: 2, md: 3 } }}
                >
                    <Grid container spacing={3}>

                        {
                            selectedField && selectedField.idCampoPadre && selectedField.idCampoPadre > 0 ? (
                                <>                                    
                                    <Grid item xs={12} sm={12}>
                                        <Autocomplete
                                            disablePortal
                                            id="campoPadre"
                                            options={parentListValuesList}
                                            isOptionEqualToValue={(option: any, value: any) => option.nombre === value.nombre} 
                                            getOptionLabel={(option) => option.nombre || ''}
                                            value={selectedParentValue || null}
                                            onChange={(event, newParentValue) => { 
                                                handleParentListValuesChange(newParentValue);                                                
                                            }}
                                            renderInput={(params) => (
                                                <TextField
                                                    {...params}
                                                    label={"* Valor de la lista predecesora: " + selectedField.nombreCampoPadre}
                                                    variant="standard"                                    
                                                />
                                            )}                      
                                        />
                                    </Grid> 
                                </>                                  
                            ) : (<></>)                            
                        }

                        <Grid item xs={12} sm={10}>
                            <TextField
                                label="* Nuevo Valor"
                                fullWidth
                                variant="standard"
                                value={newValue}
                                onChange={(event) => {
                                    setNewValue(event.target.value);
                                }}                                
                            />
                        </Grid>
                        <Grid item xs={12} sm={2}>
                            <Tooltip title={'Agregar nuevo valor a la lista'}>
                                <Fab size="small" color="primary" aria-label="add">
                                    <AddIcon onClick={handleAddValueToList} />
                                </Fab>
                            </Tooltip>                            
                        </Grid>
                        
                        <Grid item xs={12}>                                                         
                            <TableContainer component={Paper} sx={{ marginTop: 5 }}>
                                <Table stickyHeader aria-label="collapsible table" size="small">
                                <TableHead>
                                    <TableRow>                                                                        
                                        <TableCell
                                            align={"left"}
                                            style={{ minWidth: 100 }}
                                            >
                                            <b>Nombre</b>
                                        </TableCell>
                                        <TableCell
                                            align={"left"}
                                            style={{ minWidth: 100 }}
                                            >
                                            <b>Predeterminado</b>
                                        </TableCell>
                                        <TableCell
                                            align={"left"}
                                            style={{ minWidth: 100 }}
                                            >
                                            <b>Acciones</b>
                                        </TableCell>
                                    </TableRow>
                                </TableHead>
                                <TableBody>
                                    {
                                        listValuesList?.map((row, index) => (
                                            <TableRow hover key={index} sx={{}}>
                                                <TableCell>
                                                    {row.nombre}
                                                </TableCell>  
                                                <TableCell align="center">
                                                    <Checkbox 
                                                        checked={row.predeterminado}  
                                                        onChange={async (event) => { handleDefaultValue(row, event.target.checked);  }} 
                                                    />
                                                </TableCell>
                                                <TableCell sx = {{ minWidth: 160 }}>                                        
                                                    <IconButton onClick={() => {handleMoveValue(row, 'up')}} disabled={index === 0}>
                                                        <Tooltip title="Mover hacia arriba" arrow placement="top-start">
                                                            <ArrowUpwardIcon />
                                                        </Tooltip>
                                                    </IconButton>
                                                    <IconButton onClick={() => {handleMoveValue(row, 'down')}} disabled={index === listValuesList.length - 1}>
                                                        <Tooltip title="Mover hacia abajo" arrow placement="top-start">
                                                            <ArrowDownwardIcon />
                                                        </Tooltip>
                                                    </IconButton>                            
                                                    <IconButton onClick={() => {handleOpenDeleteValueDialog(row)}}>
                                                        <Tooltip title="Eliminar" arrow placement="top-start">
                                                            <DeleteIcon />
                                                        </Tooltip>
                                                    </IconButton>
                                                </TableCell>                                               
                                            </TableRow>
                                        ))
                                    }
                                </TableBody>
                                </Table>
                            </TableContainer>
                        </Grid>                       
                    </Grid>
                </Paper>
            </Box>
            </DialogContent>
            <DialogActions>
                <Box sx={{ flexGrow: 1, display: 'flex', justifyContent: 'flex-start' }}>
                    <Button
                        variant="text"
                        color="error"
                        disableElevation
                        startIcon={<DeleteIcon />}
                        onClick={() => { handleOpenDeleteAllValuesDialog(); }}
                    >
                        Eliminar valores
                    </Button>
                </Box>
                <Box sx={{ display: 'flex', justifyContent: 'flex-end' }}>
                    <Button
                        variant="outlined"
                        onClick={() => {
                        onClose(false);
                        }}
                    >
                        Cerrar
                    </Button>
                </Box>
            </DialogActions>

            <Dialog
                open={openDeleteValueDialog}
                onClose={handleCloseDeleteValueDialog}
                maxWidth={"sm"}
                >
                <AlertDialog
                    color={"error"}
                    title={"Eliminar valor de la lista"}
                    message={
                        <>
                            ¿Está seguro que desea eliminar el valor <strong>{selectedValue?.nombre}</strong>? Esta acción no se puede deshacer.
                        </>
                    }
                    onClose={handleCloseDeleteValueDialogFromAction}
                />
            </Dialog>

            <Dialog
                open={openDeleteAllValuesDialog}
                onClose={handleCloseDeleteAllValuesDialog}
                maxWidth={"sm"}
                >
                <AlertDialog
                    color={"error"}
                    title={"Eliminar todos los valores de lista"}
                    message={
                        <>
                            ¿Está seguro que desea eliminar todos los valores de la lista: <strong>{selectedField?.nombre}</strong>? Esta acción no se puede deshacer.
                        </>
                    }
                    onClose={handleCloseDeleteAllValuesDialogFromAction}
                />
            </Dialog>
        </>
    );
}
