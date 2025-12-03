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
  IconButton,
  Tooltip,
  Fab,
  Dialog,
} from "@mui/material";

// React Form Dependencies
import { useTranslation } from 'react-i18next';

// Toastr Dependencies
import { useSnackbar } from 'notistack';
import { Transicion } from '../types/Fase';
import { Proceso } from '../types/Proceso';
import DeleteIcon from '@mui/icons-material/Delete';
import AddIcon from '@mui/icons-material/Add';
import { workflowPhaseService } from '../services/settings/workflowPhaseService';
import AlertDialog from '../components/AlertDialog';

// Dialog parameters Type
type DialogProps = {
    selectedWorkflow: Proceso,
    selectedTransition: Transicion | undefined,
    onClose: (refreshSectionsList: boolean) => void
}

// Other global variables

export default function WorkflowPhaseTransitionEmailsDialog({selectedWorkflow, selectedTransition, onClose}: DialogProps) {
    // Local constants or varialbes
    const [t] = useTranslation();
    const { enqueueSnackbar } = useSnackbar();
    const [loading, setLoading] = useState<boolean>(false);
    const [transitionAssignedNotificationsList, setTransitionAssignedNotificationsList] = useState<string[]>([]);
    const [openPhaseTransitionNotificationDeleteDialog, setOpenPhaseTransitionNotificationDeleteDialog] = useState<boolean>(false);
    const [selectedTransitionNotification, setSelectedTransitionNotification] = useState<string>('');
    
    // email validation 
    const [email, setEmail] = useState<string>('');
    const [emailError, setEmailError] = useState(false);
    const [emailHelperText, setEmailHelperText] = useState("");

    const handleAddTransitionNotification = async () => {
        setLoading(true);

        try{
            if (email) {

                if(transitionAssignedNotificationsList.includes(email)) {
                    enqueueSnackbar('El correo de notificación ya se encuentra asignado a la transición.', { variant: 'warning' }); 
                    return;
                }

                const response = await workflowPhaseService.addPhaseTransitionNotification({
                    idEntidad: selectedTransition ? selectedTransition.idEntidad : 0,
                    idProceso: selectedTransition ? selectedTransition.idProceso : 0,
                    idFaseOrigen: selectedTransition ? selectedTransition.idFaseOrigen : 0,
                    idFaseDestino: selectedTransition ? selectedTransition.idFaseDestino : 0,
                    correo: email,
                    usuarioRegistro: 0,
                    fechaRegistro: new Date()
                });
    
                if(response.statusText === 'OK') {
                    fetchTransitionNotificationsData();            
                    setEmail('');                    
                    enqueueSnackbar('Correo de notifiación asignado correctamente a la trancisión.', { variant: 'success' });
                }
                else {
                    enqueueSnackbar('Ocurrió un error al asignar el correo de notificación a la transición.', { variant: 'error' });
                }            
            }
        }
        catch(error: any){
            enqueueSnackbar('Ocurrió un error al asignar el correo de notificación a la transición. Detalles: ' + error.message, { variant: 'error' });    
        }

        setLoading(false);    
    };

    const handleOpenDeleteTransitionNotificationDialog = async (selectedEmail: string) => {
        setSelectedTransitionNotification(selectedEmail);
        setOpenPhaseTransitionNotificationDeleteDialog(true);
    }       

    const handleClosePhaseTransitionNotificationDeleteDialogFromAction = async (actionResult: boolean) => {

        if(actionResult) {
            setLoading(true);

            try{
                const response = await workflowPhaseService.deletePhaseTransitionNotification(
                    selectedTransition ? selectedTransition.idProceso : 0, 
                    selectedTransition ? selectedTransition.idFaseOrigen : 0, 
                    selectedTransition ? selectedTransition.idFaseDestino : 0, 
                    selectedTransitionNotification
                );

                if(response.statusText === 'OK') {
                    fetchTransitionNotificationsData();            
                    enqueueSnackbar('Correo de notificación eliminado correctamente.', { variant: 'success' });
                }
                else {
                    enqueueSnackbar('Ocurrió un error al eliminar el correo de notifiación.', { variant: 'error' });
                }
            }
            catch(error: any){
                enqueueSnackbar('Ocurrió un error al eliminar el correo de notificación. Detalles: ' + error.message, { variant: 'error' });    
            }

            setLoading(false);
        }       

        setOpenPhaseTransitionNotificationDeleteDialog(false);
    };

    const fetchTransitionAssignedNotifications = useCallback(async () => {

        if(!selectedTransition) {
            return;
        }

        setLoading(true);

        try {
            const response = await workflowPhaseService.getPhaseTransitionNotifications(
                selectedTransition.idProceso, 
                selectedTransition.idFaseOrigen, 
                selectedTransition.idFaseDestino
            );
            
            if(response.statusText === 'OK') {
                setTransitionAssignedNotificationsList(response.data.map((tn: any) => {
                    return tn.correo
                }).sort());
            }
            else {
                enqueueSnackbar('Ocurrió un error al obtener los correos de notifiación asignados a la transición.', { variant: 'error' });
            }        
        }
        catch(error: any){
            console.log(error);
            enqueueSnackbar('Ocurrió un error al obtener los correos de notificación asignados a la transición. Detalles: ' + error.message, { variant: 'error' });            
        }

        setLoading(false);
    }, [enqueueSnackbar, selectedTransition]);

    const fetchTransitionNotificationsData = useCallback(async () => {
        
        await fetchTransitionAssignedNotifications();          
        
    }, [fetchTransitionAssignedNotifications]);

    // Dialog Actions    
    const handleClosePhaseTransitionNotificationDeleteDialog = () => {
        setOpenPhaseTransitionNotificationDeleteDialog(false);
    };
    
    const validateEmail = (value: string) => {
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/; // Simple email regex
        if (!emailRegex.test(value)) {
            setEmailError(true);
            setEmailHelperText("Correo electrónico no válido");
        } else {
            setEmailError(false);
            setEmailHelperText("");
        }
    };

    const handleEmailTextChange = (event: React.ChangeEvent<HTMLInputElement>) => {
        const value = event.target.value;
        setEmail(value);
        validateEmail(value);
    };

    useEffect (() => {        
        fetchTransitionNotificationsData();
    }, [fetchTransitionNotificationsData]);

    
    return (
        <>
            <DialogTitle>{"Configuración de Notificaciones Adicionales por Transición"}</DialogTitle>
            <DialogContent>
            <DialogContentText>
                Las notificaciones adicionales mostradas en el listado pertenecen a la transición. 
                Para agregar una notificación adicional únicamente debe escribir el correo al cual 
                desee realizar la notificación y pulsar el botón AGREGAR.
            </DialogContentText>
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
                        <Grid item xs={12} sm={10}>
                            <TextField
                                label="* Correo Electrónico"
                                fullWidth
                                variant="standard"
                                value={email}
                                onChange={handleEmailTextChange}
                                error={emailError}
                                helperText={emailHelperText}        
                            />        
                        </Grid>
                        <Grid item xs={12} sm={2}>
                            <Tooltip title={'Agregar correo electrónico'}>
                                <Fab 
                                    size="small" 
                                    color="primary" 
                                    aria-label="add" 
                                    disabled={emailError || !email}
                                >
                                    <AddIcon onClick={handleAddTransitionNotification} />
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
                                            <b>Notificación</b>
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
                                        transitionAssignedNotificationsList?.map((row, index) => (
                                            <TableRow hover key={index} sx={{}}>
                                                <TableCell>
                                                    {row}
                                                </TableCell>                                                
                                                <TableCell>                                        
                                                    <IconButton onClick={() => handleOpenDeleteTransitionNotificationDialog(row)}>
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
                open={openPhaseTransitionNotificationDeleteDialog}
                onClose={handleClosePhaseTransitionNotificationDeleteDialog}
                maxWidth={"sm"}
                >
                <AlertDialog
                    color={"error"}
                    title={"Eliminar correo de notifiación de la transición"}
                    message={
                    "Está seguro que desea eliminar el correo de notifiación seleccionado ?"
                    }
                    onClose={handleClosePhaseTransitionNotificationDeleteDialogFromAction}
                />
            </Dialog>
        </>
    );
}
