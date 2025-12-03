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
  FormGroup,
  FormControlLabel,
  Checkbox,
  Autocomplete,
} from "@mui/material";

// React Form Dependencies
import { SubmitHandler, useForm } from 'react-hook-form';
import { z } from 'zod'
import { zodResolver } from "@hookform/resolvers/zod"
import { useTranslation } from 'react-i18next';

// Toastr Dependencies
import { useSnackbar } from 'notistack';
import { Fase } from '../types/Fase';
import { Proceso } from '../types/Proceso';
import { administrativeUnitsService } from '../services/settings/administrativeUnitsService';
import { workflowPhaseService } from '../services/settings/workflowPhaseService';

// Dialog parameters Type
type DialogProps = {
    mode: 'add'|'edit',
    selectedPhase: Fase | undefined,
    selectedWorkflow: Proceso,
    onClose: (refreshSectionsList: boolean) => void
}

// Other global variables

export default function WorkflowPhaseAddEditDialog({mode, selectedPhase, selectedWorkflow, onClose}: DialogProps) {
    // Local constants or varialbes
    const [t] = useTranslation();
    const { enqueueSnackbar } = useSnackbar();
    const [loading, setLoading] = useState<boolean>(false);
    const [administrativeUnitsList, setAdministrativeUnitslist] = useState<any>([]);
    const [phaseTypesList, setPhaseTypesList] = useState<any[]>([]);
    const [phaseAccessTypesList, setPhaseAccessTypesList] = useState<any[]>([]);
    const [measurementUnitsList, setMeasurementUnitsList] = useState<any[]>([]);

    if (!selectedPhase) {
        selectedPhase = {
            idEntidad: selectedWorkflow ? selectedWorkflow.idEntidad : 0,
            idProceso: selectedWorkflow ? selectedWorkflow.idProceso : 0,
            idFase: 0,
            idTipoFase: 1,
            tipoFase: '',
            idUnidadAdministrativa: 1,
            unidadAdministrativa: '',
            nombre: '',
            descripcion: '',
            tiempoPromedio: 0,
            idUnidadMedida: 0,
            unidadMedida: '',
            asignacionObligatoria: false,
            activa: false,
            acuseRecibido: false,
            idTipoAcceso: 1,
            tipoAcceso: '',
            usuarioRegistro: 0,
            fechaRegistro: new Date(),
        };
    }

    // Form Schema definition
    const formSchema = z.object({
        idEntidad: z.number(),
        idProceso: z.number(),
        idFase: z.number(),
        idTipoFase: z.number(),
        tipoFase: z.string().min(1, t("errorMessages.requieredField")),
        idUnidadAdministrativa: z.number(),
        unidadAdministrativa: z.string().min(1, t("errorMessages.requieredField")),
        nombre: z.string().min(1, t("errorMessages.requieredField")),
        descripcion: z.string().min(1, t("errorMessages.requieredField")),
        tiempoPromedio: z.number().min(0, t("errorMessages.requieredField")), // Zod doesn't have a specific decimal type, but number suffices
        idUnidadMedida: z.number(),
        unidadMedida: z.string().min(1, t("errorMessages.requieredField")),
        asignacionObligatoria: z.boolean(),
        activa: z.boolean(),
        acuseRecibido: z.boolean(),
        idTipoAcceso: z.number(),
        tipoAcceso: z.string().min(1, t("errorMessages.requieredField")),
        usuarioRegistro: z.number(),
        fechaRegistro: z.coerce.date(), // Ensures proper date conversion
    });

    // Form Schema Type
    type UserFormType = z.infer<typeof formSchema>;

    // Form Hook
    const { register, handleSubmit, formState: { errors, isSubmitting }} = useForm<UserFormType>({
        defaultValues: selectedPhase,
        resolver: zodResolver(formSchema),
    });

    // For Submit Logic
    const onSubmit: SubmitHandler<UserFormType> = async (formData) => {
        const phaseObject: Fase = { ...formData };

        const selectedAdministrativeUnit = administrativeUnitsList.find((ua: any) => ua.label === phaseObject.unidadAdministrativa);
        phaseObject.idUnidadAdministrativa = selectedAdministrativeUnit?.id;

        const selectedPhaseType = phaseTypesList.find((pt: any) => pt.label === phaseObject.tipoFase);
        phaseObject.idTipoFase = selectedPhaseType?.id;

        const selectedPhaseAccessType = phaseAccessTypesList.find((pt: any) => pt.label === phaseObject.tipoAcceso);
        phaseObject.idTipoAcceso = selectedPhaseAccessType?.id;

        const selectedMeasurementUnit = measurementUnitsList.find((pt: any) => pt.label === phaseObject.unidadMedida);
        phaseObject.idUnidadMedida = selectedMeasurementUnit?.id;

        try {
            if (mode === "add") {               

                await workflowPhaseService.add(phaseObject);
                enqueueSnackbar("Fase creada.", { variant: "success" });

            } else {

                await workflowPhaseService.edit(phaseObject);
                enqueueSnackbar("Fase actualizada.", { variant: "success" });
            }

            onClose(true);
        } catch (error: any) {
            if (error.response?.data) {
                enqueueSnackbar(error.response.data, { variant: "error" });
            } else {
                enqueueSnackbar(error.response.data, { variant: "error" });
            }
        }
    };

    const fetchAdministrativeUnits = useCallback(async () =>{     
        try {
            const response = await administrativeUnitsService.getAll();
            if(response.statusText === 'OK') {
            setLoading(false);        
            const administrativeUnits = response.data;

            setAdministrativeUnitslist(administrativeUnits.map((ua: any) => {
                return { 
                id: ua.idUnidadAdministrativa, label: ua.nombre
                }
            }));
            }
            else {
            enqueueSnackbar('Ocurrió un Error al obtener las unidades administrativas.', { variant: 'error' });
            }        
        }
        catch(error: any){
            enqueueSnackbar('Ocurrió un Error al obtener las unidades administrativas. Detalles: ' + error.message, { variant: 'error' });
            setLoading(false);
        }
        
        return null;    
    }, [enqueueSnackbar]); 

    const fetchPhaseTypes = useCallback(async () =>{     
        try {
            const response = await workflowPhaseService.getPhaseTypes();
            if(response.statusText === 'OK') {
                setPhaseTypesList(response.data.map((pt: any) => {
                    return { 
                        id: pt.idTipoFase, label: pt.nombre
                    }
                }));
                setLoading(false);
            }
            else {
                enqueueSnackbar('Ocurrió un Error al obtener los tipos de fase.', { variant: 'error' });
            }        
        }
        catch(error: any){
            enqueueSnackbar('Ocurrió un Error al obtener lost tipos de fase. Detalles: ' + error.message, { variant: 'error' });
            setLoading(false);
        }
        
        return null;    
    }, [enqueueSnackbar]); 

    const fetchPhaseAccessTypes = useCallback(async () =>{     
        try {
            const response = await workflowPhaseService.getPhaseAccessTypes();
            if(response.statusText === 'OK') {
                setPhaseAccessTypesList(response.data.map((pt: any) => {
                    return { 
                        id: pt.idTipoAcceso, label: pt.nombre
                    }
                }));
                setLoading(false);
            }
            else {
                enqueueSnackbar('Ocurrió un Error al obtener los tipos de acceso.', { variant: 'error' });
            }        
        }
        catch(error: any){
            enqueueSnackbar('Ocurrió un Error al obtener lost tipos de acceso. Detalles: ' + error.message, { variant: 'error' });
            setLoading(false);
        }
        
        return null;    
    }, [enqueueSnackbar]); 

    const fetchMeasurementUnits = useCallback(async () =>{     
        try {
            const response = await workflowPhaseService.getMeasurementUnits();
            if(response.statusText === 'OK') {
                setMeasurementUnitsList(response.data.map((pt: any) => {
                    return { 
                        id: pt.idUnidadMedida, label: pt.nombre
                    }
                }));
                setLoading(false);
            }
            else {
                enqueueSnackbar('Ocurrió un Error al obtener las unidades de medida.', { variant: 'error' });
            }        
        }
        catch(error: any){
            enqueueSnackbar('Ocurrió un Error al obtener las unidades de medida. Detalles: ' + error.message, { variant: 'error' });
            setLoading(false);
        }
        
        return null;    
    }, [enqueueSnackbar]); 

    useEffect (() => {

        fetchAdministrativeUnits();
        fetchPhaseTypes();
        fetchPhaseAccessTypes();
        fetchMeasurementUnits();

    }, [fetchAdministrativeUnits, fetchPhaseTypes, fetchPhaseAccessTypes, fetchMeasurementUnits]);

    return (
        <>
        <form onSubmit={handleSubmit(onSubmit)}>
            <DialogTitle>{"Información de la Fase"}</DialogTitle>
            <DialogContent>
            <DialogContentText>Ingrese información de la Fase</DialogContentText>
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
                        <Autocomplete
                            disablePortal
                            id="unidadAdministrativa"
                            options={administrativeUnitsList}
                            isOptionEqualToValue={(option: any, value: any) => option.name === value.name}
                            defaultValue={selectedPhase.unidadAdministrativa}
                            renderInput={(params) => (
                                <TextField
                                    {...params}
                                    label="* Unidad Administrativa"
                                    variant="standard"
                                    {...register("unidadAdministrativa")}
                                    error = { errors.unidadAdministrativa?.message ? true : false }
                                    helperText= { errors.unidadAdministrativa?.message }
                                />
                            )}                      
                        />
                    </Grid>
                    <Grid item xs={12} sm={6}>
                        <Autocomplete
                            disablePortal
                            id="tipoFase"
                            options={phaseTypesList}
                            isOptionEqualToValue={(option: any, value: any) => option.name === value.name}
                            defaultValue={selectedPhase.tipoFase}
                            renderInput={(params) => (
                                <TextField
                                    {...params}
                                    label="* Tipo Fase"
                                    variant="standard"
                                    {...register("tipoFase")}
                                    error = { errors.tipoFase?.message ? true : false }
                                    helperText= { errors.tipoFase?.message }
                                />
                            )}                      
                        />
                    </Grid>
                    <Grid item xs={12} sm={6}>
                        <Autocomplete
                            disablePortal
                            id="tipoAcceso"
                            options={phaseAccessTypesList}
                            isOptionEqualToValue={(option: any, value: any) => option.name === value.name}
                            defaultValue={selectedPhase.tipoAcceso}
                            renderInput={(params) => (
                                <TextField
                                    {...params}
                                    label="* Tipo Acceso"
                                    variant="standard"
                                    {...register("tipoAcceso")}
                                    error = { errors.tipoAcceso?.message ? true : false }
                                    helperText= { errors.tipoAcceso?.message }
                                />
                            )}                      
                        />
                    </Grid>
                    <Grid item xs={12} sm={6}>
                        <TextField
                            label="* Nombre"
                            fullWidth
                            variant="standard"
                            {...register("nombre")}
                            error = { errors.nombre?.message ? true : false }
                            helperText= { errors.nombre?.message }
                        />
                    </Grid>
                    <Grid item xs={12} sm={6}>
                        <Box display="flex" justifyContent="flex-end">
                            {mode === "add" ? (
                            <></>
                            ) : (
                            <FormGroup>
                                <FormControlLabel
                                control={
                                    <Checkbox
                                    defaultChecked={selectedPhase?.activa}
                                    {...register("activa")}
                                    />
                                }
                                label="Activa"
                                />
                            </FormGroup>
                            )}
                        </Box>
                    </Grid>
                    <Grid item xs={12}>
                        <TextField
                            label="* Descripción"
                            fullWidth
                            variant="standard"
                            {...register("descripcion")}
                            error = { errors.descripcion?.message ? true : false }
                            helperText= { errors.descripcion?.message }
                        />
                    </Grid>
                    <Grid item xs={12} sm={4}>
                        <TextField
                            label="* Tiempo Promedio"
                            fullWidth
                            variant="standard"
                            type='number'
                            {...register("tiempoPromedio", { valueAsNumber: true })}
                            error = { errors.tiempoPromedio?.message ? true : false }
                            helperText= { errors.tiempoPromedio?.message }
                        />
                    </Grid>
                    <Grid item xs={12} sm={4}>
                        <Autocomplete
                            disablePortal
                            id="unidadMedida"
                            options={measurementUnitsList}
                            isOptionEqualToValue={(option: any, value: any) => option.name === value.name}
                            defaultValue={selectedPhase?.unidadMedida}
                            renderInput={(params) => (
                                <TextField
                                    {...params}
                                    label="* Unidad de Medida"
                                    variant="standard"
                                    {...register("unidadMedida")}
                                    error = { errors.unidadMedida?.message ? true : false }
                                    helperText= { errors.unidadMedida?.message }
                                />
                            )}                      
                        />
                    </Grid>
                    <Grid item xs={12} sm={4}>
                        <Box display="flex" flexDirection="column">
                            <FormGroup>
                            <FormControlLabel
                                control={
                                <Checkbox
                                    defaultChecked={selectedPhase.asignacionObligatoria}
                                    {...register("asignacionObligatoria")}
                                />
                                }
                                label="Asignación Obligatoria"
                            />
                            </FormGroup>
                            <FormGroup>
                            <FormControlLabel
                                control={
                                <Checkbox
                                    defaultChecked={selectedPhase.acuseRecibido}
                                    {...register("acuseRecibido")}
                                />
                                }
                                label="Acuse Recibido Obligatorio"
                            />
                            </FormGroup>
                        </Box>
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
                Cancelar
            </Button>
            <Button
                variant="contained"
                type="submit"
                disableElevation
                disabled={isSubmitting}
            >
                {isSubmitting ? "Guardando..." : "Guardar"}
            </Button>
            </DialogActions>
        </form>
        </>
    );
}
