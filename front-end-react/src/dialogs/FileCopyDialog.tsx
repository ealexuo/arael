import React from 'react'
import {
    Dialog,
    DialogTitle,
    DialogContent,
    DialogActions,
    TextField,
    Button,
    IconButton,
    Typography,
    Box,
    Grid
} from '@mui/material'
import CloseIcon from '@mui/icons-material/Close'
import { useForm, Controller, SubmitHandler } from 'react-hook-form'
import { z } from 'zod'
import { zodResolver } from '@hookform/resolvers/zod'
import { Expediente } from '../types/Expediente'
import { ContentCopy } from '@mui/icons-material'

type Props = {
    open: boolean
    file: Expediente
    onClose: () => void
    onSubmitCopias: (cantidad: number) => void
}

const schema = z.object({
    numeroCopias: z
        .number({ invalid_type_error: "Debe ingresar un número" })
        .min(1, 'Debe ser al menos 1 copia')
})

type FormType = z.infer<typeof schema>

export default function FileCopyDialog({ open, file, onClose, onSubmitCopias }: Props) {
    const {
        control,
        handleSubmit,
        formState: { errors, isSubmitting }
    } = useForm<FormType>({
        resolver: zodResolver(schema),
        defaultValues: { numeroCopias: 1 }
    })

    const formatFieldId = (fileId: number) => {
        const valueString = fileId.toString();
        const year = valueString.substring(valueString.length - 4);
        const correlative = valueString.substring(0, valueString.length - 4);
        return `${correlative}-${year}`;
    }

    const onSubmit: SubmitHandler<FormType> = async (data) => {
        // Convertir el valor a número antes de pasarlo a onSubmitCopias
        onSubmitCopias(Number(data.numeroCopias));
    }

    return (
        <Dialog open={open} onClose={onClose} fullWidth maxWidth="xs">
            <form onSubmit={handleSubmit(onSubmit)}>
                <DialogTitle sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                    <ContentCopy fontSize="small" />
                    <Box flex={1}>
                        Copiar Expediente <Typography component="span" fontWeight="normal">({formatFieldId(file.idExpediente)})</Typography>
                    </Box>
                    <IconButton onClick={onClose}>
                        <CloseIcon />
                    </IconButton>
                </DialogTitle>
                <DialogContent dividers>
                    <Typography fontWeight="bold" gutterBottom>
                        Crear Copias del Expediente
                    </Typography>
                    <Grid container spacing={2}>
                        <Grid item xs={12}>
                            <Controller
                                name="numeroCopias"
                                control={control}
                                render={({ field }) => (
                                    <TextField
                                        {...field}
                                        label="* Número de Copias"
                                        type="number"
                                        variant="standard"
                                        fullWidth
                                        error={!!errors.numeroCopias}
                                        helperText={errors.numeroCopias?.message}
                                        inputProps={{ min: 1 }}
                                        onChange={(e) => {
                                            // Convertir el valor a número antes de actualizar el estado
                                            field.onChange(parseFloat(e.target.value));
                                        }}
                                    />
                                )}
                            />
                        </Grid>
                    </Grid>
                </DialogContent>
                <DialogActions sx={{ px: 3, pb: 2 }}>
                    <Button variant="outlined" onClick={onClose}>
                        CERRAR
                    </Button>
                    <Button
                        type="submit"
                        variant="contained"
                        disabled={isSubmitting}
                    >
                        CREAR COPIAS
                    </Button>
                </DialogActions>
            </form>
        </Dialog>
    )
}
