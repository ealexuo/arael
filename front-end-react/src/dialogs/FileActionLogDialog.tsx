import React, { useRef, useState } from 'react'
import {
  Dialog, DialogTitle, DialogContent, DialogActions,
  Grid, TextField, Button, Typography, Radio, RadioGroup,
  FormControlLabel, FormLabel, IconButton, Box, Table, TableHead,
  TableRow, TableCell, TableBody, Paper
} from '@mui/material'
import DeleteIcon from '@mui/icons-material/Delete'
import CloseIcon from '@mui/icons-material/Close'
import { useForm, SubmitHandler, Controller } from 'react-hook-form'
import { z } from 'zod'
import { zodResolver } from '@hookform/resolvers/zod'
import MessageIcon from '@mui/icons-material/Message';
import AddIcon from '@mui/icons-material/Add';
import { Anotacion } from '../types/Anotacion';
import { useSnackbar } from 'notistack'
import dayjs from 'dayjs'
import AlertDialog from '../components/AlertDialog'

type Props = {
  open: boolean
  file: any
  expedienteLabel: string
  onClose: () => void
  onAddObservation: (obs: any) => void
  onDeleteObservation: (anotation: any, actionResult: boolean) => void
  anotation: Anotacion[]
}

const schema = z.object({
  privacidad: z.enum(['Público', 'Privado']),
  observacion: z.string().min(1, "Campo requerido").max(500, "Máximo 500 caracteres")
})

type FormType = z.infer<typeof schema>

export default function FileActionLogDialog({
  open,
  file,
  onClose,
  onAddObservation,
  onDeleteObservation,
  anotation
}: Props) {
  const { register, control, handleSubmit, reset, formState: { errors } } = useForm<FormType>({
    resolver: zodResolver(schema),
    defaultValues: { privacidad: 'Público', observacion: '' }
  })
  const { enqueueSnackbar } = useSnackbar()
  const [openAnotationsFileDeleteDialog, setOpenAnotationsFileDeleteDialog] = useState<boolean>(false);
  const [selectedAnotation, setSelectedAnoation] = useState<any>();

  const formatFieldId = (fileId: number) => {
    const valueString = fileId.toString();
    const year = valueString.substring(valueString.length - 4);
    const correlative = valueString.substring(0, valueString.length - 4);
    return `${correlative}-${year}`;
  }

  // Delete action dialog
  const openButtonRef = useRef<HTMLButtonElement | null>(null);
  
  const handleOpenActionFileDeleteDialog = (data: any) => {
    setSelectedAnoation(data);
    setOpenAnotationsFileDeleteDialog(true);
  }

  const handleCloseAnotationFileDeleteDialog = () => {
    setOpenAnotationsFileDeleteDialog(false);
    openButtonRef.current?.focus();
  }

  const handleCloseAnotationFileDeleteDialogFromAction = (actionResult: boolean) => {
    setOpenAnotationsFileDeleteDialog(false);
    onDeleteObservation(selectedAnotation, actionResult);
  }

  /*-------------------------------*/

  const onSubmit: SubmitHandler<FormType> = async (data) => {
    const newObservation = {
      idPrivacidad: Number(data.privacidad === 'Público' ? 1 : 2),
      observacion: data.observacion
    };

    // Llama a la función para agregar la observación
    onAddObservation(newObservation);

    // Resetea el formulario
    reset();
  }

  return (
    <>
      <Dialog open={open} onClose={onClose} maxWidth="md" fullWidth>
        <DialogTitle>
          <Box display="flex" alignItems="center" justifyContent="space-between">
            <Box display="flex" alignItems="center">
              <MessageIcon sx={{ mr: 1 }} />
              <Box display="flex" flexDirection="column">
                <Typography variant="h6" fontWeight="bold">
                  Bitácora de Acciones (Exp.{formatFieldId(file.idExpediente)})
                </Typography>
                <Typography variant="caption" color="text.secondary">
                  Los campos marcados con (*) son obligatorios.
                </Typography>
              </Box>
            </Box>
            <IconButton onClick={onClose}>
              <CloseIcon />
            </IconButton>
          </Box>
        </DialogTitle>

        <DialogContent>
          <Box mt={2}>
            <FormLabel>Privacidad</FormLabel>
            <Controller
              name="privacidad"
              control={control}
              defaultValue="Público"
              render={({ field }) => (
                <RadioGroup
                  row
                  {...field}
                  sx={{ width: '100%' }}
                >
                  <Grid container spacing={2}>
                    <Grid item xs={6} display="flex" justifyContent="center">
                      <FormControlLabel
                        value="Público"
                        control={<Radio />}
                        label="Público"
                      />
                    </Grid>
                    <Grid item xs={6} display="flex" justifyContent="center">
                      <FormControlLabel
                        value="Privado"
                        control={<Radio />}
                        label="Privado"
                      />
                    </Grid>
                  </Grid>
                </RadioGroup>
              )}
            />
          </Box>

          <Box mt={2}>
            <Controller
              name="observacion"
              control={control}
              render={({ field }) => (
                <TextField
                  {...field}
                  label="* Observación"
                  variant="standard"
                  fullWidth
                  multiline
                  minRows={1}
                  inputProps={{ maxLength: 500 }}
                  error={!!errors.observacion}
                  helperText={errors.observacion?.message}
                />
              )}
            />
          </Box>

          <Box display="flex" justifyContent="flex-end" mt={2}>
            <Button variant="contained" onClick={handleSubmit(onSubmit)}>
              <AddIcon /> AGREGAR OBSERVACIÓN
            </Button>
          </Box>

          <Box mt={4}>
            <Typography variant="subtitle2" gutterBottom>
              <b>Histórico de Mis Observaciones en la Fase Actual</b>
            </Typography>
            <Paper variant="outlined">
              <Table size="small">
                <TableHead>
                  <TableRow>
                    <TableCell align={'left'} style={{ minWidth: 100 }}><b>{'Fecha'}</b></TableCell>
                    <TableCell align={'left'} style={{ minWidth: 100 }}><b>{'Privacidad'}</b></TableCell>
                    <TableCell align={'left'} style={{ minWidth: 100 }}><b>{'Observación'}</b></TableCell>
                    <TableCell align={'center'} style={{ minWidth: 100 }}><b>{'Acción'}</b></TableCell>
                  </TableRow>
                </TableHead>
                <TableBody>
                  {anotation.map((obs, idx) => (
                    <TableRow key={idx}>
                      <TableCell>
                        {dayjs(obs.fechaRegistro).format('DD/MM/YYYY hh:mm')}
                      </TableCell>
                      <TableCell>{obs.privacidad}</TableCell>
                      <TableCell>{obs.anotacion}</TableCell>
                      <TableCell align="center">
                        {/* <IconButton onClick={() => onDeleteObservation(idx)}> */}
                        <IconButton ref={openButtonRef} onClick={() => handleOpenActionFileDeleteDialog(obs)}>
                          <DeleteIcon />
                        </IconButton>
                      </TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </Paper>
          </Box>
        </DialogContent>

        <DialogActions>
          <Button onClick={onClose} variant="outlined">
            CERRAR
          </Button>
        </DialogActions>
      </Dialog>

      <Dialog
        open={openAnotationsFileDeleteDialog}
        onClose={handleCloseAnotationFileDeleteDialog}
        maxWidth={"sm"}
      >
        <AlertDialog
          color={"error"}
          title={"Eliminar observación"}
          message={
            <>
              ¿Está seguro que desea eliminar la observación? Esta acción no se puede deshacer.
            </>
          }
          onClose={handleCloseAnotationFileDeleteDialogFromAction}
        />
      </Dialog>
    </>
  )
}
