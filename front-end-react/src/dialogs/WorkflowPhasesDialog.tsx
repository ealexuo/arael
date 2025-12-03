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
  Alert
} from '@mui/material';
import { useTranslation } from 'react-i18next';

// Toastr Dependencies
import { Proceso } from '../types/Proceso';
import WorkflowPhasesList from '../components/WorkflowPhasesList';

// Dialog parameters Type
type DialogProps = {
    selectedWorkflow: Proceso,    
    onClose: (refreshWorkflowList: boolean) => void
}

export default function WorkflowPhasesDialog({selectedWorkflow, onClose}: DialogProps) {

    // Local constants or varialbes    
    const [t] = useTranslation();
 
    return (
      <>
        <DialogTitle>
          <div style={{ display: "inline" }}>
            <div
              style={{
                display: "inline-block",
                float: "left",
                width: "2mm",
                height: "8mm",
                backgroundColor: selectedWorkflow.color,
              }}
            ></div>
            <div style={{ display: "inlineBlock", marginLeft: "21px" }}>
              {selectedWorkflow.nombre + " - Fases y Transiciones"}
            </div>
          </div>
        </DialogTitle>
        <DialogContent>
          <DialogContentText>{selectedWorkflow.descripcion}</DialogContentText>
          <Box sx={{ my: 3 }}>
            <Typography variant="subtitle1"></Typography>
            <Paper
              variant="outlined"
              sx={{ my: { xs: 2, md: 2 }, p: { xs: 2, md: 3 } }}
            >
              <Grid container spacing={3}>
                <Grid item xs={12}>
                  <Alert severity="info">
                    <div>
                      <span style={{ fontWeight: "bold" }}>Fases:</span>
                    </div>
                    <div style={{ marginTop: "8px" }}>
                      Las fases de un proceso conforman la secuencia de pasos o
                      actividades que deben realizarse con un expediente
                      perteneciente a dicho proceso. Un proceso puede contener
                      una fase incial, varias fases intermedias y una fase
                      final.
                    </div>
                    <div style={{ marginTop: "8px" }}>
                      Selecciona la fase de su interés para poder visualizarla o
                      modificarla.
                    </div>
                  </Alert>
                </Grid>
                <Grid item xs={12}>
                  <WorkflowPhasesList
                    selectedWorkflow={selectedWorkflow}
                  ></WorkflowPhasesList>
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
            Cerrar
          </Button>          
        </DialogActions>
      </>
    );
}
