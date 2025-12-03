import React from 'react'
import {
    DialogContent, DialogTitle, DialogActions, Button, Grid, TextField, Autocomplete, Box
} from "@mui/material"
import { useForm, SubmitHandler, Controller } from 'react-hook-form'
import { z } from 'zod'
import { zodResolver } from "@hookform/resolvers/zod"
import { useSnackbar } from 'notistack'
import { Proceso } from '../types/Proceso'
import { Origen } from '../types/Origen'
import { fileService } from '../services/files/fileService';

type DialogProps = {
    onClose: (refresh: boolean) => void
    procesos: Proceso[]
    origenes: Origen[]
}

type File = {
    idProceso: number;
    idOrigen: number;
    emisor: string;
    descripcion: string;
}

export default function FileCreateDialog({ onClose, procesos, origenes }: DialogProps) {
    const { enqueueSnackbar } = useSnackbar()

    const schema = z.object({
        Proceso: z.custom<Proceso>((val) => val !== null, {
            message: "Campo requerido",
        }),
        Origen: z.custom<Proceso>((val) => val !== null, {
            message: "Campo requerido",
        }),
        emisor: z.string().min(1, "Campo requerido"),
        descripcion: z.string().min(1, "Campo requerido").max(500, "Máximo 500 caracteres")
    })

    type FileFormType = {
        Proceso: Proceso | null;
        Origen: Origen | null;
        emisor: string;
        descripcion: string;
    }

    const defaultValues: FileFormType = {
        Proceso: null,
        Origen: null,
        emisor: '',
        descripcion: ''
    }

    const { control, register, handleSubmit, formState: { errors, isSubmitting } } = useForm<FileFormType>({
        resolver: zodResolver(schema),
        defaultValues,
        mode: 'onTouched'
    })

    const onSubmit: SubmitHandler<FileFormType> = async (formData) => {

        if (!formData.Proceso || !formData.Origen) return;

        const fileObject: File = {
            idProceso: formData.Proceso?.idProceso,
            idOrigen: formData.Origen?.idOrigen,
            emisor: formData.emisor,
            descripcion: formData.descripcion
        };
        try {
            await fileService.add(fileObject);
            enqueueSnackbar("Expediente creado exitosamente", { variant: "success" })
            onClose(true)
        } catch (error) {
            enqueueSnackbar("Error al crear expediente", { variant: "error" })
        }
    }

    const safeNombre = (option: any) =>
        typeof option === 'object' && option !== null && 'nombre' in option ? option.nombre : '';

    return (
        <form onSubmit={handleSubmit(onSubmit, (errors) => {
            console.log("Errores del formulario:", errors);
        })}>
            <DialogTitle>Creación del Expediente - Datos Generales</DialogTitle>
            <DialogContent>
                <Box sx={{ mt: 1, mb: 2, fontSize: 12, color: '#666' }}>
                    Los campos marcados con (*) son obligatorios.
                </Box>
                <Grid container spacing={2}>
                    <Grid item xs={12} sm={6}>
                        <Controller
                            name="Proceso"
                            control={control}
                            render={({ field }) => (
                                <Autocomplete
                                    {...field}
                                    options={procesos}
                                    getOptionLabel={safeNombre}
                                    isOptionEqualToValue={(option, value) => option.idProceso === value?.idProceso}
                                    onChange={(_, value) => field.onChange(value)}
                                    renderOption={(props, option) => (
                                        <li {...props}>
                                            <div
                                                style={{
                                                    display: 'inline-block',
                                                    width: '12px',
                                                    height: '20px',
                                                    backgroundColor: option.color || '#000',
                                                    marginRight: '8px',
                                                    borderRadius: '2px',
                                                }}
                                            ></div>
                                            {safeNombre(option)}
                                        </li>
                                    )}
                                    renderInput={(params) => (                                        
                                        <TextField
                                            {...params}
                                            label="* Proceso"
                                            variant="standard"
                                            error={!!errors.Proceso}
                                            helperText={errors.Proceso?.message || ''}
                                            fullWidth
                                        />
                                    )}
                                />
                            )}
                        />
                    </Grid>
                    <Grid item xs={12} sm={6}>
                        <Controller
                            name="Origen"
                            control={control}
                            render={({ field }) => (
                                <Autocomplete
                                    {...field}
                                    options={origenes}
                                    getOptionLabel={safeNombre}
                                    isOptionEqualToValue={(option, value) => option.idOrigen === value?.idOrigen}
                                    onChange={(_, value) => field.onChange(value)}
                                    renderInput={(params) => (
                                        <TextField
                                            {...params}
                                            label="* Origen"
                                            variant="standard"
                                            error={!!errors.Origen}
                                            helperText={errors.Origen?.message || ''}
                                            fullWidth
                                        />
                                    )}
                                />
                            )}
                        />
                    </Grid>
                    <Grid item xs={12}>
                        <TextField
                            label="Emisor"
                            error={!!errors.emisor}
                            helperText={errors.emisor?.message}
                            variant="standard"
                            fullWidth
                            inputProps={{ maxLength: 500 }} // Límite en el campo HTML
                            {...register("emisor", {
                                maxLength: {
                                    value: 500,
                                    message: "El emisor no puede tener más de 500 caracteres",
                                },
                            })}
                        />
                    </Grid>
                    <Grid item xs={12}>
                        <TextField
                            label="Descripción"
                            error={!!errors.emisor}
                            helperText={errors.emisor?.message}
                            variant="standard"
                            fullWidth
                            multiline
                            {...register("descripcion")}
                        />
                    </Grid>
                </Grid>
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
    )
}
