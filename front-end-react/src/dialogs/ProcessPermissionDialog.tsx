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
    Divider,
    Checkbox,
} from "@mui/material";

import { useTranslation } from 'react-i18next';

// Services Dependencies
import { processPermissionService } from '../services/settings/processPermissionService';

// Toastr Dependencies
import { useSnackbar } from 'notistack';

import { TextField } from '@mui/material';

// Dialog parameters Type
type DialogProps = {
    selectedUser: any,
    processPermissionList: any,
    onClose: (refreshUsersList: boolean) => void
}

export default function ProcessPermissionDialog({ selectedUser, processPermissionList, onClose }: DialogProps) {

    // Local constants or varialbes    
    const [t] = useTranslation();
    const { enqueueSnackbar } = useSnackbar();
    const [permissionList, setPermissionList] = useState<any[]>(processPermissionList)

    const handlePermissionChange = (idProceso: number, idEntidad: number, idPermiso: number) => {
        let permissionListTemp = [...permissionList];

        const processIndex = permissionListTemp.findIndex((item: any) => item.proceso.idProceso === idProceso && item.proceso.idEntidad === idEntidad);
        const permissionIndex = permissionListTemp[processIndex].listaPermisos.findIndex((item: any) => item.idPermiso === idPermiso);

        permissionListTemp[processIndex].listaPermisos[permissionIndex].habilitado = !permissionListTemp[processIndex].listaPermisos[permissionIndex].habilitado;
        setPermissionList(permissionListTemp);
    }

    const [filterText, setFilterText] = useState('');

    // Form Submit Logic
    const onSubmit = async () => {

        try {
            await processPermissionService.edit(permissionList, selectedUser);
            enqueueSnackbar("Se asignaron los permisos seleccionados.", { variant: "success" });
            onClose(true);
        } catch (error: any) {
            const errorMessage = error.response && error.response.data ? error.response.data : error.message;
            enqueueSnackbar('Ocurrión un error al asignar los permisos seleccionados. Detalles: ' + errorMessage, { variant: 'error' });
        }

    }

    return (
        <>
            <DialogTitle>{"Asignación de permisos por proceso"}</DialogTitle>
            <DialogContent>
                <DialogContentText>Busque el proceso de su interés y seleccione los permisos a asignar al usuario.</DialogContentText>
                <Box sx={{ my: 3 }}>
                    <TextField
                        label="Buscar proceso"
                        variant="outlined"
                        fullWidth
                        value={filterText}
                        onChange={(e) => setFilterText(e.target.value)}
                        sx={{ mb: 3 }}
                    />
                    <Typography variant="subtitle1"></Typography>
                    <Paper
                        variant="outlined"
                        sx={{ my: { xs: 2, md: 2 }, p: { xs: 2, md: 3 } }}
                    >
                        {permissionList
                            .filter((item: any) =>
                                item.proceso.nombre.toLowerCase().includes(filterText.toLowerCase())
                            )
                            .map((item: any, index: number) => {
                                return (
                                    <Grid container spacing={3} key={index}>
                                        <Grid item xs={12} sm={12}>
                                            <Typography variant="h6">{item.proceso.nombre}</Typography>
                                        </Grid>
                                        <Grid item xs={12} sm={12}>
                                            {item.listaPermisos?.map((permission: any, index: number) => {
                                                return (
                                                    <React.Fragment key={index}>
                                                        <Grid container style={{ color: 'gray' }}>
                                                            <Grid item sm={3}>
                                                                {permission.nombre}
                                                            </Grid>
                                                            <Grid item sm={7}>
                                                                {permission.descripcion}
                                                            </Grid>
                                                            <Grid item sm={2} style={{ textAlign: 'center' }}>
                                                                <Checkbox
                                                                    checked={permission.habilitado}
                                                                    onChange={() => {
                                                                        handlePermissionChange(
                                                                            item.proceso.idProceso,
                                                                            item.proceso.idEntidad,
                                                                            permission.idPermiso
                                                                        );
                                                                    }}
                                                                />
                                                            </Grid>
                                                        </Grid>
                                                        <Divider style={{ marginBottom: 21 }} />
                                                    </React.Fragment>
                                                );
                                            })}
                                        </Grid>
                                        <hr />
                                    </Grid>
                                );
                            })}

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


