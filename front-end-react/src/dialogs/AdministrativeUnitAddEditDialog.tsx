import React from 'react'

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
    Autocomplete
} from "@mui/material";

// React Form Dependencies
import { SubmitHandler, useForm } from 'react-hook-form';
import { number, z } from 'zod'
import { zodResolver } from "@hookform/resolvers/zod"
import { useTranslation } from 'react-i18next';

// Toastr Dependencies
import { useSnackbar } from 'notistack';
import { UnidadAdministrativa } from '../types/UnidadAdministrativa';
import { administrativeUnitsService } from '../services/settings/administrativeUnitsService';


// Dialog parameters Type
type DialogProps = {
    mode: 'add' | 'edit',
    selectedAdministrativeUnit: UnidadAdministrativa
    administrativeUnitsList: UnidadAdministrativa[],
    onClose: (refreshAdministrativeUnitsList: boolean) => void
}

// Other global variables

export default function AdministrativeUnitAddEditDialog({ mode, selectedAdministrativeUnit, administrativeUnitsList, onClose }: DialogProps) {
 
    // Local constants or varialbes    
    const [t] = useTranslation();
    const { enqueueSnackbar } = useSnackbar();

    const [selectedAdministrativeUnitFather, setSelectedAdministrativeUnitFather] = React.useState<number>(selectedAdministrativeUnit.idUnidadAdministrativaPadre ? selectedAdministrativeUnit.idUnidadAdministrativaPadre : 0);
    
    const selectedAdministrativeUnitDefault: UnidadAdministrativa = {
        idUnidadAdministrativa: selectedAdministrativeUnit.idUnidadAdministrativa,
        idUnidadAdministrativaPadre: selectedAdministrativeUnit.idUnidadAdministrativaPadre ? selectedAdministrativeUnit.idUnidadAdministrativaPadre : 0,
        nombreUnidadAdministrativaPadre: selectedAdministrativeUnit.nombreUnidadAdministrativaPadre ? selectedAdministrativeUnit.nombreUnidadAdministrativaPadre : "",
        nombre: selectedAdministrativeUnit.nombre,
        siglas: selectedAdministrativeUnit.siglas ? selectedAdministrativeUnit.siglas : "",
        activa: selectedAdministrativeUnit.activa,
    }

    // Form Schema definition
    const formSchema = z.object({
        idUnidadAdministrativa: z.number(),
        idUnidadAdministrativaPadre: z.number(),
        nombreUnidadAdministrativaPadre: z.string(),
        nombre: z.string(),
        siglas: z.string(),
        activa: z.boolean()
    });

    // Form Schema Type
    type AdministrativeUnitFormType = z.infer<typeof formSchema>;

    // Form Hook
    const { register, handleSubmit, formState: { errors, isSubmitting } } = useForm<AdministrativeUnitFormType>({
        defaultValues: selectedAdministrativeUnitDefault,
        resolver: zodResolver(formSchema),
    });

    // For Submit Logic
    const onSubmit: SubmitHandler<AdministrativeUnitFormType> = async (formData) => {

        let administrativeUnitObject: UnidadAdministrativa = { ...formData };
        
        try {

            if (mode === "add") {

                await administrativeUnitsService.add(administrativeUnitObject);
                enqueueSnackbar("Unidad Administrativa creada.", { variant: "success" });

            } else {

                administrativeUnitObject.idUnidadAdministrativaPadre = selectedAdministrativeUnitFather;
                const response = await administrativeUnitsService.edit(administrativeUnitObject);

                if (response.data !== -1) {
                    enqueueSnackbar("Unidad Administrativa actualizada.", { variant: "success" });
                }

            }

            onClose(true);

        } catch (error: any) {

            if (error.response?.data) {
                enqueueSnackbar(error.response.data, { variant: "error" });
            } else {
                enqueueSnackbar(error.response.data, { variant: "error" });
            }

        }
    }

    return (
        <>
            <form onSubmit={handleSubmit(onSubmit)}>
                <DialogTitle>{"Información de la Unidad Administrativa"}</DialogTitle>
                <DialogContent>
                    <DialogContentText>Ingrese información general de la Unidad Administratriva</DialogContentText>
                    <Box sx={{ my: 3 }}>
                        <Typography variant="subtitle1">
                        </Typography>
                        <Paper
                            variant="outlined"
                            sx={{ my: { xs: 2, md: 2 }, p: { xs: 2, md: 3 } }}
                        >
                            <Grid container spacing={3}>
                                <Grid item xs={12} sm={12}>
                                    <Autocomplete                                        
                                        id="unidadAdministrativa"
                                        options={administrativeUnitsList}
                                        isOptionEqualToValue={(option: any, value: any) => option.idUnidadAdministrativa === value.idUnidadAdministrativa}
                                        defaultValue={administrativeUnitsList.find((item) => item.idUnidadAdministrativa === selectedAdministrativeUnit.idUnidadAdministrativaPadre) }
                                        onChange={(event, newValue) => {                                            
                                            setSelectedAdministrativeUnitFather(newValue ? newValue.idUnidadAdministrativa : 0);
                                        }}                                       
                                        renderInput={(params) => (
                                            <TextField
                                                {...params}
                                                label="* Unidad Administrativa Padre"
                                                variant="standard"
                                                fullWidth
                                            />
                                        )}
                                        getOptionLabel={(option) => option.nombre}
                                    />                                   
                                </Grid>
                                <Grid item xs={12} sm={12}>
                                    <TextField
                                        label="* Unidad Administrativa"
                                        fullWidth
                                        variant="standard"
                                        {...register("nombre")}
                                        error={errors.nombre?.message ? true : false}
                                        helperText={errors.nombre?.message}
                                    />
                                </Grid>
                                <Grid item xs={12} sm={12}>
                                    <Grid item xs={4} sm={4}>
                                        <TextField
                                            label="Siglas"
                                            fullWidth
                                            variant="standard"
                                            {...register("siglas")}
                                            error={errors.siglas?.message ? true : false}
                                            helperText={errors.siglas?.message}
                                        />
                                    </Grid>
                                    <Box display="flex" justifyContent="flex-end">
                                        {
                                            mode === 'add' ?
                                                <></> :
                                                <FormGroup>
                                                    <FormControlLabel
                                                        control={
                                                            <Checkbox
                                                                defaultChecked={selectedAdministrativeUnit.activa}
                                                                {...register("activa")}
                                                            />
                                                        }
                                                        label="Activo"
                                                    />
                                                </FormGroup>
                                        }
                                    </Box>

                                </Grid>
                            </Grid>
                        </Paper>
                    </Box>
                </DialogContent>
                <DialogActions>
                    <Button variant="outlined" onClick={() => { onClose(false) }}>
                        Cancelar
                    </Button>
                    <Button variant="contained" type="submit" disableElevation disabled={isSubmitting}>
                        {isSubmitting ? "Guardando..." : "Guardar"}
                    </Button>
                </DialogActions>
            </form>
        </>
    )
}
