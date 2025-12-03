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

export default function WorkflowPhaseTransitionUsersDialog({selectedWorkflow, selectedTransition, onClose}: DialogProps) {
    // Local constants or varialbes
    const [t] = useTranslation();
    const { enqueueSnackbar } = useSnackbar();
    const [loading, setLoading] = useState<boolean>(false);
    const [phaseUsersList, setPhaseUsersList] = useState<any[]>([]);
    const [transitionAssignedUsersList, setTransitionAssignedUsersList] = useState<any[]>([]);
    const [selectedUser, setSelectedUser] = useState<any>();
    const [openPhaseTransitionUserDeleteDialog, setOpenPhaseTransitionUserDeleteDialog] = useState<boolean>(false);
    const [selectedAssignedUserId, setSelectedAssignedUserId] = useState<number>(0);

    const handleAddAssignedUser = async () => {
        setLoading(true);

        try{
            if (selectedUser) {
                const response = await workflowPhaseService.addPhaseTransitionUser({
                    idEntidad: selectedTransition ? selectedTransition.idEntidad : 0,
                    idProceso: selectedTransition ? selectedTransition.idProceso : 0,
                    idFaseOrigen: selectedTransition ? selectedTransition.idFaseOrigen : 0,
                    idFaseDestino: selectedTransition ? selectedTransition.idFaseDestino : 0,
                    idUsuario: selectedUser.id,
                    recepcionTraslado: false,
                    nombre: selectedUser.label,
                    usuarioRegistro: 0,
                    fechaRegistro: new Date(),
                });
    
                if(response.statusText === 'OK') {
                    fetchUsersData();            
                    setSelectedUser(null);                    
                    enqueueSnackbar('Usuario asignado a la trancisión correctamente.', { variant: 'success' });
                }
                else {
                    enqueueSnackbar('Ocurrió un error al asignar el usuario a la transición.', { variant: 'error' });
                }            
            }
        }
        catch(error: any){
            enqueueSnackbar('Ocurrió un error al asignar el usuario a la transición. Detalles: ' + error.message, { variant: 'error' });    
        }

        setLoading(false);    
    };

    const handleDeleteTransitionAssignedUser = async (userId: number) => {
        setSelectedAssignedUserId(userId);
        setOpenPhaseTransitionUserDeleteDialog(true);
    }
    
    const fetchPhaseUsers = useCallback(async (workflowId: number, phaseId: number) => {
        setLoading(true);

        try {
            const response = await workflowPhaseService.getPhaseUsers(workflowId, phaseId);
            if(response.statusText === 'OK') {
                setPhaseUsersList(response.data.map((u: any) => {
                    return { 
                        id: u.idUsuario, label: u.nombre
                    }
                }).sort((a: any, b: any) => a.label.localeCompare(b.label)));
            }
            else {
                enqueueSnackbar('Ocurrió un error al obtener los usuarios de la fase.', { variant: 'error' });
            }        
        }
        catch(error: any){
            enqueueSnackbar('Ocurrió un error al obtener los usuarios de la fase. Detalles: ' + error.message, { variant: 'error' });
        }

        setLoading(false);
    }, [enqueueSnackbar]);

    const fetchTransitionAssignedUsers = useCallback(async () => {

        if(!selectedTransition) {
            return;
        }

        setLoading(true);

        try {
            const response = await workflowPhaseService.getPhaseTransitionUsers(
                selectedTransition.idProceso, 
                selectedTransition.idFaseOrigen, 
                selectedTransition.idFaseDestino
            );
            
            if(response.statusText === 'OK') {
                setTransitionAssignedUsersList(response.data.map((u: any) => {
                    return { 
                        idUsuario: u.idUsuario, nombre: u.usuario
                    }
                }).sort((a: any, b: any) => a.nombre.localeCompare(b.nombre)));
            }
            else {
                enqueueSnackbar('Ocurrió un error al obtener los usuarios asignados a la transición.', { variant: 'error' });
            }        
        }
        catch(error: any){
            enqueueSnackbar('Ocurrió un error al obtener los usuarios asignados a la transición. Detalles: ' + error.message, { variant: 'error' });            
        }

        setLoading(false);
    }, [enqueueSnackbar, selectedTransition]);

    const fetchUsersData = useCallback(async () => {
        
        await fetchPhaseUsers(selectedTransition ? selectedTransition.idProceso : 0, selectedTransition ? selectedTransition.idFaseOrigen : 0);        
        await fetchTransitionAssignedUsers();          
        
    }, [fetchPhaseUsers, fetchTransitionAssignedUsers, selectedTransition]);

    // Dialog Actions    
    const handleClosePhaseTransitionUserDeleteDialog = () => {
        setOpenPhaseTransitionUserDeleteDialog(false);
    };

    const handleClosePhaseTransitionUserDeleteDialogFromAction = async (actionResult: boolean) => {
        if(actionResult) {
            setLoading(true);

            try{
                const response = await workflowPhaseService.deletePhaseTransitionUser(
                    selectedTransition ? selectedTransition.idProceso : 0, 
                    selectedTransition ? selectedTransition.idFaseOrigen : 0, 
                    selectedTransition ? selectedTransition.idFaseDestino : 0, 
                    selectedAssignedUserId
                );

                if(response.statusText === 'OK') {
                    fetchUsersData();            
                    enqueueSnackbar('Usuario desasignado correctamente.', { variant: 'success' });
                }
                else {
                    enqueueSnackbar('Ocurrió un error al desasignar al usuario.', { variant: 'error' });
                }
            }
            catch(error: any){
                enqueueSnackbar('Ocurrió un error al desasignar al usuario. Detalles: ' + error.message, { variant: 'error' });    
            }

            setLoading(false);
        }       

        setOpenPhaseTransitionUserDeleteDialog(false);
    };

    useEffect (() => {        
        fetchUsersData();
    }, [fetchUsersData]);

    useEffect(() => {

        setPhaseUsersList((prevUsers) =>
            prevUsers.filter(
                (user) => !transitionAssignedUsersList.some((assigned) => assigned.idUsuario === user.id)
            )
        );
        
    }, [transitionAssignedUsersList]); 

    return (
        <>
            <DialogTitle>{"Configuración de usarios por transición"}</DialogTitle>
            <DialogContent>
            <DialogContentText>Los usuarios mostrados en el listado pertenecen a la transición. Para asociar a un usaurio debe buscarlo por nombre.</DialogContentText>
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
                            <Autocomplete
                                disablePortal
                                id="usuario"
                                options={phaseUsersList}
                                isOptionEqualToValue={(option: any, value: any) => option.name === value.name} 
                                getOptionLabel={(option) => option.label || ''}
                                value={selectedUser || null}
                                onChange={(event, newValue) => { 
                                    setSelectedUser(newValue);
                                }}
                                renderInput={(params) => (
                                    <TextField
                                        {...params}
                                        label="* Usuario"
                                        variant="standard"                                    
                                    />
                                )}                      
                            />
                        </Grid>              
                        <Grid item xs={12} sm={2}>
                            <Tooltip title={'Asignar Usuario a la Transición'}>
                                <Fab size="small" color="primary" aria-label="add">
                                    <AddIcon onClick={handleAddAssignedUser} />
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
                                            <b>Acciones</b>
                                        </TableCell>
                                    </TableRow>
                                </TableHead>
                                <TableBody>
                                    {
                                        transitionAssignedUsersList?.map((row, index) => (
                                            <TableRow hover key={index} sx={{}}>
                                                <TableCell>
                                                    {row.nombre}
                                                </TableCell>                                                
                                                <TableCell>                                        
                                                    <IconButton onClick={() => handleDeleteTransitionAssignedUser(row.idUsuario)}>
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
                open={openPhaseTransitionUserDeleteDialog}
                onClose={handleClosePhaseTransitionUserDeleteDialog}
                maxWidth={"sm"}
                >
                <AlertDialog
                    color={"error"}
                    title={"Desasignar usuario de la tarnsición"}
                    message={
                    "Está seguro que desea desasignar al usuario seleccionado ?"
                    }
                    onClose={handleClosePhaseTransitionUserDeleteDialogFromAction}
                />
            </Dialog>
        </>
    );
}
