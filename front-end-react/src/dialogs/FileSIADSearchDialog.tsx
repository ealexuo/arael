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

type DialogProps = {
    onClose: (refresh: boolean) => void    
}

// type File = {
//     idProceso: number;
//     idOrigen: number;
//     emisor: string;
//     descripcion: string;
// }

export default function FileSIADSearchDialog({ onClose }: DialogProps) {
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
            <DialogTitle>Consulta SIAD</DialogTitle>
            <DialogContent>                
                <Grid container spacing={2} sx={{ width: 1250 }}>
                    <Grid item xs={12} sm={6}>
                        <TextField
                            label="Número de Entrada SIAD"                            
                            variant="standard"
                            fullWidth
                            value={248600}
                            type='number'
                            inputProps={{ maxLength: 500 }} 
                            disabled={true}                           
                        />                        
                    </Grid>
                    <Grid item xs={12} sm={6}>
                        <TextField
                            label="Llave"                            
                            variant="standard"
                            value={'P2L234'}
                            fullWidth
                            type='text'
                            inputProps={{ maxLength: 500 }}  
                            disabled={true}                          
                        />
                    </Grid>
                    <Grid item xs={12}>
                       <iframe
                            title="Consulta SIAD"
                            src="https://siadreg.mspas.gob.gt/consulta/"
                            width="100%"
                            height="400px"
                            style={{ border: 'none' }}
                        />
                    </Grid>                    
                </Grid>
            </DialogContent>
            <DialogActions>
                <Button variant="outlined" onClick={() => { onClose(false) }}>
                    Cerrar
                </Button>
            </DialogActions>
        </form>
    )
}
