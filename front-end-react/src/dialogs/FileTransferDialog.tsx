import React, { useEffect, useState } from 'react';
import {
  Dialog, DialogTitle, DialogContent, DialogActions,
  TextField, Typography, IconButton, Button, Box, Grid, Autocomplete
} from '@mui/material';
import CloseIcon from '@mui/icons-material/Close';
import { useForm, Controller, SubmitHandler, useWatch, useFormContext } from 'react-hook-form';
import { z } from 'zod';
import { zodResolver } from '@hookform/resolvers/zod';
import { CompareArrows } from '@mui/icons-material';

// Tipos
type Fase = {
  idFase: number;
  fase: string;
  listaUsuarios: Usuario[];
  idTipoFase: number;
  asignacionObligatoria: boolean;
};

type Usuario = {
  idUsuario: number;
  usuario: string | null;
};

// Props
type Props = {
  open: boolean;
  file: {
    idExpediente: number;
    asignadoPor: string;
    fechaAsignacion: string; // formato legible
    fechaLimite?: string; // puede ser "No Definida"
    observacion?: string;
    porcentajeTiempoTranscurrido?: number;
  };
  onClose: () => void;
  onSubmit: (data: any) => void;
  fases: Fase[];
};

// Validación
const schema = z.object({
  faseDestino: z.object({ idFase: z.number(), fase: z.string(), listaUsuarios: z.array(z.any()), idTipoFase: z.number(), asignacionObligatoria: z.boolean() }, { required_error: 'Campo requerido' }),
  usuarioAsignado: z.object({
    idUsuario: z.number(),
    usuario: z.string().nullable()
  }, { required_error: 'Campo requerido' }).optional(),
  fechaLimiteAtencion: z
    .string()
    .optional()
    .refine((value) => {
      if (!value) return true;
      // parse local: YYYY-MM-DD -> Date(y, m-1, d) (medianoche local)
      const [y, m, d] = value.split('-').map(Number);
      const selected = new Date(y, m - 1, d);

      const today = new Date();
      today.setHours(0, 0, 0, 0); // inicio del día local

      return selected >= today;  // permite HOY y posteriores
    }, {
      message: 'La fecha no puede ser menor a hoy',
    }),
  observaciones: z.string().min(1, 'Campo requerido'),
});

type FormType = z.infer<typeof schema>;

export default function FileTransferDialog({ open, file, onClose, onSubmit, fases }: Props) {
  const {
    control,
    handleSubmit,
    formState: { errors },
    setValue,
    register
  } = useForm<FormType>({
    resolver: zodResolver(schema),
    defaultValues: {
      faseDestino: undefined,
      usuarioAsignado: undefined,
      fechaLimiteAtencion: '',
      observaciones: '',
    },
  });

  const [usuarios, setUsuarios] = useState<Usuario[]>([]);
  const [camposDeshabilitados, setCamposDeshabilitados] = useState(false);

  const faseSeleccionada = useWatch({
    control,
    name: 'faseDestino',
  });

  useEffect(() => {
    if (!faseSeleccionada) return;

    const fase = fases.find(f => f.idFase === faseSeleccionada.idFase);
    if (!fase) return;

    // ⚠️ Limpiar selección anterior de usuario (aunque el campo ya tenga undefined)
    setValue('usuarioAsignado', undefined, { shouldValidate: true, shouldDirty: true });

    if (fase.idTipoFase === 3) {
      setCamposDeshabilitados(true);
      setUsuarios([]);
      setValue('fechaLimiteAtencion', '');
    } else {
      let lista = [...fase.listaUsuarios];

      lista = lista.filter(u => u.idUsuario !== -1);
      if (lista.length && lista[0].usuario === null) lista.shift();

      if (!fase.asignacionObligatoria) {
        lista.push({ idUsuario: -1, usuario: 'Asignación Libre' });
      }

      setUsuarios(lista);
      setCamposDeshabilitados(false);
    }
  }, [faseSeleccionada, fases, setValue]);

  const handleFormSubmit: SubmitHandler<FormType> = (data) => {
    onSubmit(data);
  };

  const formatFieldId = (fileId: number) => {
    const valueString = fileId.toString();
    const year = valueString.substring(valueString.length - 4);
    const correlative = valueString.substring(0, valueString.length - 4);
    return `${correlative}-${year}`;
  }
  return (
    <Dialog open={open} onClose={onClose} maxWidth="sm" fullWidth>
      <DialogTitle>
        <Box display="flex" alignItems="center" justifyContent="space-between">
          <Box display="flex" alignItems="center">
            <CompareArrows />
            <Box display="flex" flexDirection="column">
              <Typography variant="h6" fontWeight="bold">
                Traslado de Expediente (Exp. {formatFieldId(file.idExpediente)})
              </Typography>
              <Typography variant="caption" color="text.secondary">
                Traslado de Expediente a otra Fase.
              </Typography>
            </Box>
          </Box>
          <IconButton onClick={onClose}>
            <CloseIcon />
          </IconButton>
        </Box>
      </DialogTitle>

      <form onSubmit={handleSubmit(handleFormSubmit)}>
        <DialogContent>
          <Grid container spacing={2}>
            <Grid item xs={12}>
              <Controller
                name="faseDestino"
                control={control}
                render={({ field }) => (
                  <Autocomplete
                    {...field}
                    options={fases}
                    getOptionLabel={(option) => option.fase}
                    isOptionEqualToValue={(option, value) => option.idFase === value?.idFase}
                    onChange={(_, value) => field.onChange(value)}
                    renderInput={(params) => (
                      <TextField
                        {...params}
                        label="* Fase Destino"
                        variant="standard"
                        fullWidth
                        error={!!errors.faseDestino}
                        helperText={errors.faseDestino?.message}
                      />
                    )}
                  />
                )}
              />
            </Grid>

            <Grid item xs={12}>
              <Controller
                name="usuarioAsignado"
                control={control}
                render={({ field }) => (
                  <Autocomplete
                    key={faseSeleccionada?.idFase ?? 'default'} // 🔁 clave dinámica para forzar reinicio visual
                    {...field}
                    options={usuarios}
                    getOptionLabel={(option) => option.usuario ?? ''}
                    isOptionEqualToValue={(option, value) => option.idUsuario === value?.idUsuario}
                    onChange={(_, value) => field.onChange(value)}
                    disabled={camposDeshabilitados}
                    renderInput={(params) => (
                      <TextField
                        {...params}
                        label="* Asignar a:"
                        variant="standard"
                        fullWidth
                        error={!!errors.usuarioAsignado}
                        helperText={errors.usuarioAsignado?.message}
                      />
                    )}
                  />
                )}
              />
            </Grid>

            <Grid item xs={12}>
              <Controller
                name="fechaLimiteAtencion"
                control={control}
                render={({ field }) => (
                  <TextField
                    {...field}
                    label="Fecha Límite de Atención"
                    variant="standard"
                    type="date"
                    InputLabelProps={{ shrink: true }}
                    fullWidth
                    disabled={camposDeshabilitados}
                    error={!!errors.fechaLimiteAtencion}
                    helperText={errors.fechaLimiteAtencion?.message}
                    inputProps={{ min: new Date().toISOString().split('T')[0] }}
                  />
                )}
              />
            </Grid>

            <Grid item xs={12}>
              <Controller
                name="observaciones"
                control={control}
                render={({ field }) => (
                  <TextField
                    {...field}
                    label="* Observaciones"
                    variant="standard"
                    fullWidth
                    multiline
                    minRows={2}
                    inputProps={{ maxLength: 500 }}
                    error={!!errors.observaciones}
                    helperText={errors.observaciones?.message}
                  />
                )}
              />
            </Grid>
          </Grid>
        </DialogContent>

        <DialogActions>
          <Button variant="contained" type="submit" >
            TRASLADAR
          </Button>
          <Button onClick={onClose} variant="outlined">
            CERRAR
          </Button>
        </DialogActions>
      </form>
    </Dialog>
  );
}
