import React from 'react'
import {
    DialogContent, DialogTitle, DialogActions, Button, Grid, TextField, Autocomplete, Box,
    ToggleButtonGroup,
    ToggleButton
} from "@mui/material"
import { useForm, SubmitHandler, Controller } from 'react-hook-form'
import { z } from 'zod'
import { zodResolver } from "@hookform/resolvers/zod"
import { useSnackbar } from 'notistack'
import { Proceso } from '../types/Proceso'
import { Origen } from '../types/Origen'
import { fileService } from '../services/files/fileService';
import { DatePicker, LocalizationProvider } from '@mui/x-date-pickers'
import { AdapterMoment } from '@mui/x-date-pickers/AdapterMoment'

type DialogProps = {
    onClose: (refresh: boolean) => void    
}

// type File = {
//     idProceso: number;
//     idOrigen: number;
//     emisor: string;
//     descripcion: string;
// }

export default function FileCreateNoteDialog({ onClose }: DialogProps) {
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

        // if (!formData.Proceso || !formData.Origen) return;

        // const fileObject: File = {
        //     idProceso: formData.Proceso?.idProceso,
        //     idOrigen: formData.Origen?.idOrigen,
        //     emisor: formData.emisor,
        //     descripcion: formData.descripcion
        // };
        // try {
        //     await fileService.add(fileObject);
        //     enqueueSnackbar("Expediente creado exitosamente", { variant: "success" })
        //     onClose(true)
        // } catch (error) {
        //     enqueueSnackbar("Error al crear expediente", { variant: "error" })
        // }
    }

    const safeNombre = (option: any) =>
        typeof option === 'object' && option !== null && 'nombre' in option ? option.nombre : '';


    const [alignment, setAlignment] = React.useState('web');

    const handleToggleButtonChange = (
        event: React.MouseEvent<HTMLElement>,
        newAlignment: string,
    ) => {
        setAlignment(newAlignment);
    };



    return (
        <form onSubmit={handleSubmit(onSubmit, (errors) => {
            console.log("Errores del formulario:", errors);
        })}>
            <DialogTitle>Datos de la Nota</DialogTitle>
            <DialogContent>
                <Box sx={{ mt: 1, mb: 2, fontSize: 12, color: '#666' }}>
                    Los campos marcados con (*) son obligatorios.
                </Box>
                <Grid container spacing={2}>
                    {/* <Grid item xs={12} sm={12}>
                        <ToggleButtonGroup
                            color="primary"
                            value={alignment}
                            exclusive
                            onChange={handleToggleButtonChange}
                            aria-label="Platform"
                            >
                            <ToggleButton value="web">Laboratorio Nacional de Salud (LNS)</ToggleButton>
                            <ToggleButton value="android">Ministry of Health (MOH)</ToggleButton>                            
                        </ToggleButtonGroup>
                    </Grid> */}
                    <Grid item xs={12} sm={6}>
                         <LocalizationProvider dateAdapter={AdapterMoment}>
                            <DatePicker
                            views={['year', 'month', 'day']}
                            label={'Fecha de recepción'}
                            slotProps={{ textField: { variant: "standard", fullWidth: true } }}
                            />
                        </LocalizationProvider>                    
                    </Grid>
                    <Grid item xs={12} sm={6}>
                        <TextField
                            label="Páginas"                            
                            variant="standard"
                            fullWidth
                            type='text'
                            inputProps={{ maxLength: 500 }}                            
                        />
                    </Grid>
                    <Grid item xs={12}>
                        <TextField
                            label="Descripción"                            
                            variant="standard"
                            multiline
                            fullWidth
                            type='text'
                            inputProps={{ maxLength: 500 }}                            
                        />
                    </Grid>
                    <Grid item xs={12} sm={6}>
                        <TextField
                            label="Meses para corrección"                            
                            variant="standard"
                            fullWidth
                            type='number'
                            inputProps={{ maxLength: 500 }}                            
                        />
                    </Grid>
                </Grid>
            </DialogContent>
            <DialogActions>
                <Button variant="outlined" onClick={() => { onClose(false) }}>
                    Cancelar
                </Button>
                <Button variant="contained" type="submit" disableElevation disabled={isSubmitting}>
                    {isSubmitting ? "Guardando..." : "Crear"}
                </Button>
            </DialogActions>
        </form>
    )
}
