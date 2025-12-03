import React from 'react'
import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  Typography,
  Box
} from '@mui/material'
import WarningAmberIcon from '@mui/icons-material/WarningAmber'
import { FormatAlignJustify } from '@mui/icons-material'

type UnifyDialogProps = {
  open: boolean
  file : any
  onClose: () => void
  onConfirm: () => void
}

export default function FileUnifyDialog({ open, file, onClose, onConfirm }: UnifyDialogProps) {
  return (
    <Dialog open={open} onClose={onClose} maxWidth="xs" fullWidth>        
      <DialogTitle>Unificar Expedientes</DialogTitle>

      <DialogContent sx={{ textAlign: 'center' }}>
        <Box sx={{ my: 2 }}>
          <WarningAmberIcon sx={{ fontSize: 100, color: '#f5a623' }} />
        </Box>

        <Typography variant="subtitle1" sx={{ fontWeight: 600 }}>
          ¿Está seguro de Unificar los Expedientes?
        </Typography>

        <Typography
          variant="body2"
          color="textSecondary"
          sx={{ mt: 1, px: 2, lineHeight: 1.5 }}
        >
          Esta acción dejará activo únicamente el expediente más antiguo y archivará
          los expedientes más recientes.
          <br /><br />
          Una vez realizada la unificación no podrá revertir la operación.
        </Typography>
      </DialogContent>

      <DialogActions sx={{ justifyContent: 'center', pb: 3 }}>
        <Button
          variant="outlined"
          onClick={onClose}          
        >
          CANCELAR
        </Button>

        <Button
          variant="contained"
          onClick={onConfirm}         
        >
          SÍ, UNIFICAR
        </Button>
      </DialogActions>
    </Dialog>
  )
}
