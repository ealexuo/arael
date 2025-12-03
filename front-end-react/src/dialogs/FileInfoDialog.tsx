import React from 'react';
import {
    DialogTitle,
    DialogContent,
    DialogActions,
    Button,
    Typography,
    Box,
    Grid,
    Icon,
    IconButton,
} from '@mui/material';
import InfoIcon from '@mui/icons-material/Info';
import dayjs from 'dayjs';
import CircleIcon from '@mui/icons-material/Circle';
import CloseIcon from '@mui/icons-material/Close'

type AssignmentInfoProps = {
    onClose: () => void;
    file: {
        idExpediente: number;
        asignadoPor: string;
        fechaAsignacion: string; // formato legible
        fechaLimite?: string; // puede ser "No Definida"
        observacion?: string;
        porcentajeTiempoTranscurrido?: number;
    };
};

const formatFieldId = (fileId: number) => {
    const valueString = fileId.toString();
    const year = valueString.substring(valueString.length - 4);
    const correlative = valueString.substring(0, valueString.length - 4);
    return `${correlative}-${year}`;
}

export default function FileInfoDialog({ onClose, file }: AssignmentInfoProps) {
    return (
        <>
            <DialogTitle>
                <Box display="flex" alignItems="center" justifyContent="space-between">
                    <Box display="flex" alignItems="center">
                        <InfoIcon sx={{ mr: 1 }} />
                        <Box display="flex" flexDirection="column">
                            <Typography variant="h6" fontWeight="bold">
                            Información de la Asignación (Exp. {formatFieldId(file.idExpediente)})
                            </Typography>
                            <Typography variant="caption" color="text.secondary">
                            Información de la asignación realizada a tu usuario.
                            </Typography>
                        </Box>
                    </Box>
                    <IconButton onClick={onClose}>
                        <CloseIcon />
                    </IconButton>
                </Box>
            </DialogTitle>
            <DialogContent dividers>
                <Grid container spacing={2}>
                    <Grid item xs={12} sm={6}>
                        <Typography variant="body2" color="textSecondary">
                            Asignado Por:
                        </Typography>
                        <Typography>{file.asignadoPor}</Typography>
                    </Grid>

                    <Grid item xs={12} sm={6}>
                        <Typography variant="body2" color="textSecondary">
                            Fecha de Asignación:
                        </Typography>
                        <Typography>{dayjs(file.fechaAsignacion).format('DD/MM/YYYY')}</Typography>
                    </Grid>

                    <Grid item xs={12} sm={6}>
                        <Typography variant="body2" color="textSecondary">
                            Fecha de Límite de Atención:
                        </Typography>
                        <Typography>
                            {dayjs(file.fechaLimite).format('DD/MM/YYYY') || 'No Definida'}
                        </Typography>
                    </Grid>
                    <Grid item xs={12} sm={6}>
                        <Typography variant="body2" color="textSecondary">
                            Semáforo:
                        </Typography>
                        <span>
                            {(file.porcentajeTiempoTranscurrido ?? 0) === -1 && (
                                <CircleIcon sx={{ color: theme => theme.palette.success.main }} />
                            )}

                            {(file.porcentajeTiempoTranscurrido ?? 0) >= 0 &&
                                (file.porcentajeTiempoTranscurrido ?? 0) < 75 && (
                                    <CircleIcon sx={{ color: theme => theme.palette.success.main }} />
                                )}

                            {(file.porcentajeTiempoTranscurrido ?? 0) >= 75 &&
                                (file.porcentajeTiempoTranscurrido ?? 0) <= 90 && (
                                    <CircleIcon sx={{ color: theme => theme.palette.warning.main }} />
                                )}

                            {((file.porcentajeTiempoTranscurrido ?? 0) >= 90 ||
                                (file.porcentajeTiempoTranscurrido ?? 0) < -1) && (
                                    <CircleIcon sx={{ color: theme => theme.palette.error.main }} />
                                )}
                        </span>
                    </Grid>
                    <Grid item xs={12}>
                        <Typography variant="body2" color="textSecondary">
                            Observación de Asignación:
                        </Typography>
                        <Typography>{file.observacion || '—'}</Typography>
                    </Grid>
                </Grid>
            </DialogContent>

            <DialogActions>
                <Button variant="contained" onClick={onClose}>
                    CERRAR
                </Button>
            </DialogActions>
        </>
    );
}
