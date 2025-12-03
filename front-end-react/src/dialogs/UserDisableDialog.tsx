import React, { useState } from 'react'

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
    Alert,
} from "@mui/material";

import { useTranslation } from 'react-i18next';

// Services Dependencies
import { userDisableService } from '../services/settings/userDisableService';

// Toastr Dependencies
import { useSnackbar } from 'notistack';

import moment from 'moment';
import 'moment/locale/es';
import UserDisablesList from '../components/UserDisablesList';

// Dialog parameters Type
type DialogProps = {
    mode: 'add' | 'edit',
    selectedUser: any,
    onClose: (refreshUsersList: boolean) => void
}

// Other global variables
moment.locale('es-gt');

export default function UserDisableDialog({ mode, selectedUser, onClose }: DialogProps) {

    // Local constants or varialbes    
    const [t] = useTranslation();
    const { enqueueSnackbar } = useSnackbar();
    const [initialDate, setInitialDate] = useState<any>(moment());
    const [finalDate, setFinalDate] = useState<any>(moment());

    const nombreCompleto = [selectedUser.primerNombre,
    selectedUser.segundoNombre,
    selectedUser.tercerNombre,
    selectedUser.primerApellido,
    selectedUser.segundoApellido,
    selectedUser.apellidoCasada].join(' ').replace(/\s+/g, ' ');

    // For Submit Logic
    const onSubmit = async () => {

        const disableUserObject = {
            idHistoricoInhabilitacion: 0,
            idUsuario: selectedUser.idUsuario,
            idEntidad: selectedUser.idEntidad,
            fechaInicio: initialDate,
            fechaFin: finalDate
        }

        try {
            await userDisableService.add(disableUserObject);
            enqueueSnackbar("Inhabilitación creada.", { variant: "success" });
            onClose(true);
        } catch (error: any) {
            const errorMessage = error.response && error.response.data ? error.response.data : error.message;
            enqueueSnackbar('Ocurrión un error al ihabilitar al usuario. Detalles: ' + errorMessage, { variant: 'error' });
        }
    }

    return (
        <>
            <DialogTitle>{"Inhabilitaciones del Usuario: " + nombreCompleto}</DialogTitle>
            <DialogContent>
                <DialogContentText>{"Identificador Personal: " + selectedUser.noIdentificacionPersonal}</DialogContentText>
                <Box sx={{ my: 3 }}>
                    <Typography variant="subtitle1"></Typography>
                    <Paper
                        variant="outlined"
                        sx={{ my: { xs: 2, md: 2 }, p: { xs: 2, md: 3 } }}
                    >
                        <Grid container spacing={3}>
                            <Grid item xs={12}>
                                <Alert severity="info">
                                    <div>
                                        <span style={{ fontWeight: "bold" }}>Inhabilitación:</span>
                                    </div>
                                    <div style={{ marginTop: "8px" }}>
                                        Al inhabilitar temporalmente a un usuario, no será posible asignarle expedientes durante el periodo de inhabilitación.
                                    </div>
                                </Alert>
                            </Grid>                           
                            <Grid item xs={12}>
                                <UserDisablesList
                                    selectedUser={selectedUser}
                                ></UserDisablesList>
                            </Grid>
                        </Grid>
                    </Paper>
                </Box>
            </DialogContent>
            <DialogActions>
                <Button variant="outlined" onClick={() => { onClose(false) }}>
                    Cancelar
                </Button>
                <Button variant="contained" onClick={onSubmit} disableElevation>
                    {"Guardar"}
                </Button>
            </DialogActions>
        </>
    )
}


