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
  TableContainer,
  Table,
  TableHead,
  TableRow,
  TableCell,
  TableBody,
  Tooltip,
  Fab,
  Dialog,
  FormGroup,
  FormControlLabel,
  Checkbox,
  IconButton,
} from "@mui/material";

// React Form Dependencies
import { useTranslation } from 'react-i18next';

// Toastr Dependencies
import { useSnackbar } from 'notistack';
import { Transicion } from '../types/Fase';
import { Proceso } from '../types/Proceso';
import AddIcon from '@mui/icons-material/Add';
import { workflowPhaseService } from '../services/settings/workflowPhaseService';
import AlertDialog from '../components/AlertDialog';
import FieldTypeSelect from '../components/FieldTypeSelect';
import DeleteIcon from '@mui/icons-material/Delete';


// Dialog parameters Type
type DialogProps = {
    selectedWorkflow: Proceso,
    selectedTransition: Transicion | undefined,
    onClose: (refreshSectionsList: boolean) => void
}

// Other global variables

export default function WorkflowPhaseTransitionRequirementsDialog({selectedWorkflow, selectedTransition, onClose}: DialogProps) {
    // Local constants or varialbes
    const [t] = useTranslation();
    const { enqueueSnackbar } = useSnackbar();
    const [loading, setLoading] = useState<boolean>(false);
    const [transitionRequierimentsList, setTransitionRequierimentsList] = useState<any[]>([]);
    const [openPhaseTransitionRequierimentDeleteDialog, setOpenPhaseTransitionRequierimentDeleteDialog] = useState<boolean>(false);
    const [selectedFieldType, setSelectedFieldType] = useState<any>();
    const [selectedFieldName, setSelectedFieldName] = useState<string>('');
    const [isRequiered, setIsRequired] = useState<boolean>(false);
    const [selectedRequeriment, setSelectedRequieriment] = useState<any>();

    const handleFieldTypeChange = (newValue: any) => {
        setSelectedFieldType(newValue);
    }

    const handleAddRequirement = async () => {

        setLoading(true);

        try{
            if (selectedFieldName !== '' && selectedFieldType) {
                const response = await workflowPhaseService.addPhaseTransitionRequieriment({
                    idEntidad: selectedTransition ? selectedTransition.idEntidad : 0,
                    idProceso: selectedTransition ? selectedTransition.idProceso : 0,
                    idFaseOrigen: selectedTransition ? selectedTransition.idFaseOrigen : 0,
                    idFaseDestino: selectedTransition ? selectedTransition.idFaseDestino : 0,
                    idRequisito: 0,
                    idTipoCampo: selectedFieldType.idTipoCampo,
                    nombreTipoCampo: selectedFieldType.nombre,
                    campo: selectedFieldName,
                    obligatorio: isRequiered,
                    usuarioRegistro: 0,
                    fechaRegistro: new Date(),
                });
    
                if(response.statusText === 'OK') {
                    fetchTransitionRequierimentsData();            
                    setSelectedFieldType(null);
                    setSelectedFieldName('');
                    setIsRequired(false);
                    enqueueSnackbar('Requisito agregado a la trancisión correctamente.', { variant: 'success' });
                }
                else {
                    enqueueSnackbar('Ocurrió un error al agregar el requisito a la transición.', { variant: 'error' });
                }            
            }
        }
        catch(error: any){
            enqueueSnackbar('Ocurrió un error al agregar el requisito a la transición. Detalles: ' + error.message, { variant: 'error' });    
        }

        setLoading(false);    
    };

    const handleDeleteTransitionRequieriment = async (requieriment: any) => {
        setSelectedRequieriment(requieriment);
        setOpenPhaseTransitionRequierimentDeleteDialog(true);
    }
    
    const fetchTransitionRequieriments = useCallback(async () => {

        if(!selectedTransition) {
            return;
        }
       
        setLoading(true);

        try {
            const response = await workflowPhaseService.getPhaseTransitionRequieriments(
                selectedTransition.idProceso, 
                selectedTransition.idFaseOrigen, 
                selectedTransition.idFaseDestino
            );
            
            if(response.statusText === 'OK') {
                setTransitionRequierimentsList(response.data.map((r: any) => {
                    return { 
                        id: r.idRequisito,
                        name: r.campo,
                        type: r.nombreTipoCampo,
                        required: r.obligatorio                        
                    }
                }).sort((a: any, b: any) => a.name.localeCompare(b.name)));
            }
            else {
                enqueueSnackbar('Ocurrió un error al obtener los requisitos agregados a la transición.', { variant: 'error' });
            }        
        }
        catch(error: any){
            enqueueSnackbar('Ocurrió un error al obtener los requisitos agregados a la transición. Detalles: ' + error.message, { variant: 'error' });            
        }

        setLoading(false);
    }, [enqueueSnackbar, selectedTransition]);


    // Dialog Actions    
    const handleClosePhaseTransitionRequierimentDeleteDialog = () => {
        setOpenPhaseTransitionRequierimentDeleteDialog(false);
    };

    const handleClosePhaseTransitionRequierimentDeleteDialogFromAction = async (actionResult: boolean) => {
        if(actionResult) {
            setLoading(true);

            try{
                const response = await workflowPhaseService.deletePhaseTransitionRequieriment(
                    selectedTransition ? selectedTransition.idProceso : 0, 
                    selectedTransition ? selectedTransition.idFaseOrigen : 0, 
                    selectedTransition ? selectedTransition.idFaseDestino : 0, 
                    selectedRequeriment ? selectedRequeriment.id : 0
                );

                if(response.statusText === 'OK') {
                    fetchTransitionRequierimentsData();            
                    enqueueSnackbar('Requisito eliminado correctamente.', { variant: 'success' });
                }
                else {
                    enqueueSnackbar('Ocurrió un error al eliminar el requisito.', { variant: 'error' });
                }
            }
            catch(error: any){
                enqueueSnackbar('Ocurrió un error al eliminar el requisito. Detalles: ' + error.message, { variant: 'error' });    
            }

            setLoading(false);
        }       

        setOpenPhaseTransitionRequierimentDeleteDialog(false);
    };

    const fetchTransitionRequierimentsData = useCallback(async () => {
        await fetchTransitionRequieriments();
    }, [fetchTransitionRequieriments]);

    useEffect (() => {        
        fetchTransitionRequierimentsData();
    }, [fetchTransitionRequierimentsData]);

    return (
        <>
            <DialogTitle>{"Requisitos por Transición"}</DialogTitle>
            <DialogContent>
            <DialogContentText>Los requisitos mostrados en el listado pertenencen a la transición.</DialogContentText>
            <Box sx={{ my: 3 }}>
                <Typography variant="subtitle1"></Typography>
                <Paper
                variant="outlined"
                sx={{ my: { xs: 2, md: 2 }, p: { xs: 2, md: 3 } }}
                >
                    <Grid container spacing={3}>
                        <Grid item xs={12} sm={6}>
                            <TextField
                                label="Proceso"
                                fullWidth
                                variant="standard"
                                value={selectedWorkflow?.nombre}
                                disabled={true}                        
                            />
                        </Grid>
                        <Grid item xs={12} sm={6}>
                            <TextField
                                label="Unidad Administrativa"
                                fullWidth
                                variant="standard"
                                value = {selectedTransition?.unidadAdministrativaFO}
                                disabled={true}                        
                            />
                        </Grid>                    
                        <Grid item xs={12} sm={6}>
                            <TextField
                                label="Fase Origen"
                                fullWidth
                                variant="standard"
                                value = {selectedTransition?.faseOrigen}
                                disabled={true}                        
                            />
                        </Grid>
                        <Grid item xs={12} sm={6}>
                            <TextField
                                label="Fase Destino"
                                fullWidth
                                variant="standard"
                                value = {selectedTransition?.faseDestino}
                                disabled={true}                        
                            />
                        </Grid>
                        <Grid item xs={12} sm={6}>
                            <FieldTypeSelect 
                                onSelectedFieldTypeChange={handleFieldTypeChange} 
                            />
                        </Grid>
                        <Grid item xs={12} sm={6}>
                            <TextField
                                label="* Nombre"
                                fullWidth
                                variant="standard"
                                onChange={(e) => setSelectedFieldName(e.target.value)}
                                value={selectedFieldName}
                            />
                        </Grid> 
                        <Grid item xs={12} sm={6}>
                            <FormGroup>
                                <FormControlLabel
                                    control={
                                        <Checkbox
                                            checked={isRequiered}
                                            onChange={(e) => setIsRequired(e.target.checked)}
                                            name="isRequiered"
                                        />
                                    }
                                    label="Obligatorio"
                                />
                            </FormGroup>        
                        </Grid>                         
                        <Grid item xs={12} sm={6} style={{ textAlign: 'right' }}>
                            <Tooltip title={'Asignar Usuario a la Transición'}>
                                <Fab 
                                    size="small" 
                                    color="primary" 
                                    aria-label="add"
                                    disabled={!selectedFieldType || selectedFieldName === ''}
                                    >
                                    <AddIcon onClick={handleAddRequirement} />
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
                                            <b>Tipo</b>
                                        </TableCell>
                                        <TableCell
                                            align={"left"}
                                            style={{ minWidth: 100 }}
                                            >
                                            <b>Obligatorio</b>
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
                                        transitionRequierimentsList?.map((row, index) => (
                                            <TableRow hover key={index} sx={{}}>
                                                <TableCell>
                                                    {row.name}
                                                </TableCell>  
                                                <TableCell>
                                                    {row.type}
                                                </TableCell>  
                                                <TableCell>
                                                    <Checkbox
                                                        checked={row.required}
                                                        disabled={true}                                                                    
                                                    />
                                                </TableCell>
                                                <TableCell>                                        
                                                    <IconButton onClick={() => handleDeleteTransitionRequieriment(row)}>
                                                        <DeleteIcon />
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
                <Button
                    variant="outlined"
                    onClick={() => {
                    onClose(false);
                    }}
                >
                    Cerrar
                </Button>            
            </DialogActions>

            <Dialog
                open={openPhaseTransitionRequierimentDeleteDialog}
                onClose={handleClosePhaseTransitionRequierimentDeleteDialog}
                maxWidth={"sm"}
                >
                <AlertDialog
                    color={"error"}
                    title={"Eliminar requerimiento de la transición"}
                    message={
                    "Está seguro que desea eliminar el requerimiento seleccionado ?"
                    }
                    onClose={handleClosePhaseTransitionRequierimentDeleteDialogFromAction}
                />
            </Dialog>
        </>
    );
}
