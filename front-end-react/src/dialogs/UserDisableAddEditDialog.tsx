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
} from "@mui/material";

import { useTranslation } from 'react-i18next';

import { SubmitHandler, useForm } from 'react-hook-form';
import { z } from 'zod'

import { zodResolver } from "@hookform/resolvers/zod"

// Services Dependencies
import { userDisableService } from '../services/settings/userDisableService';

// Toastr Dependencies
import { useSnackbar } from 'notistack';

// Date Picker Dependencies
import { AdapterMoment } from '@mui/x-date-pickers/AdapterMoment';
import { LocalizationProvider } from '@mui/x-date-pickers/LocalizationProvider';
import { DatePicker } from '@mui/x-date-pickers/DatePicker';

import moment from 'moment';
import 'moment/locale/es';
import { Inhabilitacion } from '../types/Inhabilitacion';

// Dialog parameters Type
type DialogProps = {
    mode: 'add' | 'edit',
    selectedUser: any,
    selectedDisable: any,
    onClose: (refreshUsersList: boolean) => void
}


// Other global variables
moment.locale('es-gt');

export default function UserDisableAddEditDialog({ mode, selectedUser, selectedDisable, onClose }: DialogProps) {

    // Local constants or varialbes    
    const [t] = useTranslation();
    const { enqueueSnackbar } = useSnackbar();

    // ... existing code ...
    const [initialDate, setInitialDate] = useState<moment.Moment>(
        selectedDisable ? moment(selectedDisable.fechaInicio) : moment()
    );
    const [finalDate, setFinalDate] = useState<moment.Moment>(
        selectedDisable ? moment(selectedDisable.fechaFin) : moment()
    );

    React.useEffect(() => {
        setInitialDate(selectedDisable ? moment(selectedDisable.fechaInicio) : moment());
        setFinalDate(selectedDisable ? moment(selectedDisable.fechaFin) : moment());
    }, [selectedDisable]);

    // Form Schema definition
    const formSchema = z.object({
        idHistoricoInhabilitacion: z.number(),
        idUsuario: z.number(),
        idEntidad: z.number(),
        fechaInicio: z.date(),
        fechaFin: z.date(),
        fechaRegistro: z.date(),
        usuarioRegistro: z.number()
    });

    // Form Schema Type
    type DisableFormType = z.infer<typeof formSchema>;

    const initialDisable: DisableFormType = {
        idHistoricoInhabilitacion: selectedDisable?.idHistoricoInhabilitacion ?? 0,
        idUsuario: selectedUser.idUsuario,
        idEntidad: selectedUser.idEntidad,
        fechaInicio: initialDate.toDate(),
        fechaFin: finalDate.toDate(),
        fechaRegistro: moment().toDate(),
        usuarioRegistro: selectedUser.idUsuario
    };

    // Form Hook
    const { register, handleSubmit, formState: { errors, isSubmitting } } = useForm<DisableFormType>({
        defaultValues: initialDisable,
        resolver: zodResolver(formSchema),
    });


    const nombreCompleto = [selectedUser.primerNombre,
    selectedUser.segundoNombre,
    selectedUser.tercerNombre,
    selectedUser.primerApellido,
    selectedUser.segundoApellido,
    selectedUser.apellidoCasada].join(' ').replace(/\s+/g, ' ');

    // For Submit Logic
    const onSubmit: SubmitHandler<DisableFormType> = async (formData) => {

        const disableUserObject: Inhabilitacion = {
            ...formData,
            idUsuario: selectedUser.idUsuario,
            fechaInicio: initialDate.toDate(),
            fechaFin: finalDate.toDate(),
            fechaRegistro: moment().toDate(),
            idHistoricoInhabilitacion: selectedDisable?.idHistoricoInhabilitacion ?? 0,
            usuarioRegistro: selectedUser.idUsuario
        };

        try {

            if (mode === "add") {                
                await userDisableService.add(disableUserObject);
                enqueueSnackbar("Inhabilitación creada.", { variant: "success" });
                onClose(true);

            }
            else {
                console.log("EDIT ",disableUserObject);
                await userDisableService.edit(disableUserObject);
                enqueueSnackbar("Inhabilitación actualizada.", { variant: "success" });
            }

            onClose(true);

        } catch (error: any) {
            const errorMessage = error.response && error.response.data ? error.response.data : error.message;
            enqueueSnackbar('Ocurrión un error al registrar la inhabilitación del usuario. Detalles: ' + errorMessage, { variant: 'error' });
        }


    }

    return (
        <>
            <DialogTitle>{"Inhabilitación de Usuario"}</DialogTitle>
            <DialogContent>
                <DialogContentText>Al inhabilitar temporalmente a un usuario no será posible asignarle expedientes durante el periodo de inhabilitación.</DialogContentText>
                <Box sx={{ my: 3 }}>
                    <Typography variant="subtitle1"></Typography>
                    <Paper
                        variant="outlined"
                        sx={{ my: { xs: 2, md: 2 }, p: { xs: 2, md: 3 } }}
                    >
                        <Grid container spacing={3}>
                            <Grid item xs={12} sm={6}>
                                <TextField
                                    label="Identificación Personal"
                                    fullWidth
                                    disabled={true}
                                    variant="standard"
                                    value={selectedUser.noIdentificacionPersonal}
                                />
                            </Grid>
                            <Grid item xs={12} sm={6}>
                                <TextField
                                    label="Correo Electrónico"
                                    fullWidth
                                    disabled={true}
                                    variant="standard"
                                    value={selectedUser.correoElectronico}
                                />
                            </Grid>
                            <Grid item xs={12} sm={12}>
                                <TextField
                                    label="Nombre Completo"
                                    fullWidth
                                    disabled={true}
                                    variant="standard"
                                    value={nombreCompleto}
                                />
                            </Grid>
                            <LocalizationProvider dateAdapter={AdapterMoment}>
                                <Grid item xs={12} sm={6}>
                                    <DatePicker
                                        views={['year', 'month', 'day']}
                                        label="* Fecha Inicial"
                                        name="fechaInicial"
                                        value={initialDate}
                                        slotProps={{ textField: { variant: "standard", fullWidth: true } }}
                                        onChange={(newDate) => setInitialDate(newDate || moment())}
                                    />
                                </Grid>
                                <Grid item xs={12} sm={6}>
                                    <DatePicker
                                        views={['year', 'month', 'day']}
                                        label="* Fecha Final"
                                        name="fechaFinal"
                                        value={finalDate}
                                        slotProps={{ textField: { variant: "standard", fullWidth: true } }}
                                        onChange={(newDate) => setFinalDate(newDate || moment())}
                                    />
                                </Grid>
                            </LocalizationProvider>
                        </Grid>
                    </Paper>
                </Box>
            </DialogContent>
            <DialogActions>
                <Button variant="outlined" onClick={() => { onClose(false) }}>
                    Cancelar
                </Button>
                <Button
                    variant="contained"
                    onClick={handleSubmit(onSubmit)}
                    disabled={isSubmitting}
                    disableElevation
                >
                    {mode === 'add' ? 'Guardar' : 'Actualizar'}
                </Button>
            </DialogActions>
        </>
    )
}