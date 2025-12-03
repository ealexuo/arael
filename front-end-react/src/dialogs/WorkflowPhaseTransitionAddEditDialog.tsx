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
import { Fase, Transicion } from '../types/Fase';
import { Proceso } from '../types/Proceso';
import { workflowPhaseService } from '../services/settings/workflowPhaseService';

import EastIcon from '@mui/icons-material/East';

// Dialog parameters Type
type DialogProps = {
    mode: 'add'|'edit',
    selectedPhaseName: string | undefined,
    selectedWorkflow: Proceso,
    phasesList: Fase[] | undefined,
    selectedTransition: Transicion | undefined,
    onClose: (refreshSectionsList: boolean) => void
}

// Other global variables

export default function WorkflowPhaseTransitionAddEditDialog({
    mode, 
    selectedPhaseName, 
    selectedWorkflow, 
    phasesList,
    selectedTransition,
    onClose
}: DialogProps) {
    // Local constants or varialbes
    const [t] = useTranslation();
    const { enqueueSnackbar } = useSnackbar();
    const [loading, setLoading] = useState<boolean>(false);
    // const [administrativeUnitsList, setAdministrativeUnitslist] = useState<any>([]);

    const originPhaseTemp = phasesList?.find((p: any) => p.nombre === selectedPhaseName);

    const filteredPhasesList = phasesList?.filter((phase: Fase) => {
        const isNotInTransitionList = !originPhaseTemp?.listaTransiciones?.some(
            (transition: Transicion) => transition.idFaseDestino === phase.idFase
        );
        const isNotSelectedPhase = phase.nombre !== selectedPhaseName;
        const isCurrentDestinationFase = selectedTransition?.faseDestino === phase.nombre;

        return (isNotInTransitionList && isNotSelectedPhase) || isCurrentDestinationFase;
    }).map((phase: Fase) => phase.nombre) || [];

    if (!selectedTransition) {
        selectedTransition = {
            idEntidad: selectedWorkflow?.idEntidad || 0,
            idProceso: selectedWorkflow?.idProceso || 0,
            idFaseOrigen: originPhaseTemp?.idFase || 0,
            faseOrigen: originPhaseTemp?.nombre || '',
            unidadAdministrativaFO: originPhaseTemp?.unidadAdministrativa || '',
            idFaseDestino: 0,
            faseDestino: '',
            unidadAdministrativaFD: originPhaseTemp?.unidadAdministrativa || '',
            activa: false,
            usuarioRegistro: 0,
            fechaRegistro: new Date()
        };
    }

    // Form Schema definition
    const formSchema = z.object({
        idEntidad: z.number(),
        idProceso: z.number(),
        idFaseOrigen: z.number(),
        faseOrigen: z.string(),
        unidadAdministrativaFO: z.string(),
        idFaseDestino: z.number(),
        faseDestino: z.string().min(1, t("errorMessages.requieredField")),
        unidadAdministrativaFD: z.string(),
        activa: z.boolean(),
        usuarioRegistro: z.number(),
        fechaRegistro: z.coerce.date(),
    });       

    // Form Schema Type
    type UserFormType = z.infer<typeof formSchema>;

    // Form Hook
    const { register, handleSubmit, formState: { errors, isSubmitting }} = useForm<UserFormType>({
        defaultValues: selectedTransition,
        resolver: zodResolver(formSchema),
    });

    // For Submit Logic
    const onSubmit: SubmitHandler<UserFormType> = async (formData) => {
        
        const transitionObject: Transicion = { ...formData };        
        const selectedDestinationPhase = phasesList?.find((p: any) => p.nombre === transitionObject.faseDestino);
        transitionObject.idFaseDestino = selectedDestinationPhase?.idFase || 0;

        try {
            if (mode === "add") {               

                await workflowPhaseService.addPhaseTransition(transitionObject);
                enqueueSnackbar("Transición creada.", { variant: "success" });

            } else {

                await workflowPhaseService.activatePhaseTransition(transitionObject);
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

    // const fetchAdministrativeUnits = useCallback(async () =>{     
    //     try {
    //         const response = await administrativeUnitsService.getAll();
    //         if(response.statusText === 'OK') {
    //         setLoading(false);        
    //         const administrativeUnits = response.data;

    //         setAdministrativeUnitslist(administrativeUnits.map((ua: any) => {
    //             return { 
    //             id: ua.idUnidadAdministrativa, label: ua.nombre
    //             }
    //         }));
    //         }
    //         else {
    //         enqueueSnackbar('Ocurrió un Error al obtener las unidades administrativas.', { variant: 'error' });
    //         }        
    //     }
    //     catch(error: any){
    //         enqueueSnackbar('Ocurrió un Error al obtener las unidades administrativas. Detalles: ' + error.message, { variant: 'error' });
    //         setLoading(false);
    //     }
        
    //     return null;    
    // }, [enqueueSnackbar]);
    

    useEffect (() => {

        //fetchAdministrativeUnits();
        
    }, []);

    return (
        <>
        <form onSubmit={handleSubmit(onSubmit)}>
            <DialogTitle>{"Información de la Transición"}</DialogTitle>
            <DialogContent>
            <DialogContentText>Ingrese información de la Transición</DialogContentText>
            <Box sx={{ my: 3 }}>
                <Typography variant="subtitle1"></Typography>
                <Paper
                variant="outlined"
                sx={{ my: { xs: 2, md: 2 }, p: { xs: 2, md: 3 } }}
                >
                <Grid container spacing={3}>
                    <Grid item xs={12}>
                        <TextField
                            label="Proceso"
                            fullWidth
                            variant="standard"
                            value={selectedWorkflow?.nombre}
                            disabled={true}                        
                        />
                    </Grid>
                    <Grid item xs={5}>
                        <TextField
                            label="Fase Origen"
                            fullWidth
                            variant="standard"
                            value={selectedPhaseName}
                            disabled={true}                        
                        />
                    </Grid>
                    <Grid 
                        item 
                        xs={2} 
                        container 
                        justifyContent="center" 
                        alignItems="center" 
                        color={"grey.500"}
                    >
                        <EastIcon />
                    </Grid>
                    <Grid item xs={5}>
                        <Autocomplete
                            disabled={mode === "edit"}
                            disablePortal
                            id="phasesList"
                            options={filteredPhasesList}
                            isOptionEqualToValue={(option: any, value: any) => option.name === value.name}
                            value={selectedTransition?.faseDestino}
                            renderInput={(params) => (
                                <TextField
                                    {...params}
                                    label="* Fase Destino"
                                    variant="standard"
                                    {...register("faseDestino")}
                                    error = { errors.faseDestino?.message ? true : false }
                                    helperText= { errors.faseDestino?.message }
                                />
                            )}                      
                        />
                    </Grid>
                    <Grid item xs={12}>
                        <Box display="flex" justifyContent="flex-end">
                            {mode === "add" ? (
                            <></>
                            ) : (
                            <FormGroup>
                                <FormControlLabel
                                control={
                                    <Checkbox
                                    defaultChecked={selectedTransition?.activa}
                                    {...register("activa")}
                                    />
                                }
                                label="Activa"
                                />
                            </FormGroup>
                            )}
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
