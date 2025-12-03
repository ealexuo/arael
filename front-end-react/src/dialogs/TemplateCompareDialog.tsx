import React, { useEffect, useCallback } from 'react'

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
  Alert,
} from "@mui/material";

import { useTranslation } from 'react-i18next';

// Toastr Dependencies
import { useSnackbar } from 'notistack';
import { Plantilla } from '../types/Plantilla';
import { Proceso } from '../types/Proceso';
import SectionsListViewer from '../components/SectionsListViewer';

// Dialog parameters Type
type DialogProps = {
  originalTemplate: Plantilla | undefined,
  selectedTemplate: Plantilla | undefined,
  selectedWorkflow: Proceso,
  onClose: (refreshWorkflowTemplatesList: boolean) => void
}

// Other global variables

export default function TemplateCompareDialog({ originalTemplate, selectedTemplate, selectedWorkflow, onClose }: DialogProps) {

  // Local constants or varialbes    
  const [t] = useTranslation();
  const { enqueueSnackbar } = useSnackbar();

  if (!selectedTemplate) {

  }

  // Fetch functions
  const fetchTemplates = useCallback(async (workflowId: number, currentTemplate: Plantilla | undefined) => {

    /* try {
      const response = await templateService.getAll(workflowId);
      if (response.statusText === "OK") {
        const templatesListTemp = [...response.data];
        
        setTemplatesList(templatesListTemp);
        setTemplatesNamesList(templatesListTemp.map((t: any) => {
          return { label: t.nombre + (t.activa ? ' (Activa)' : ''), id: t.idPlantilla };
        }));
 
        if(currentTemplate) {
          const selectedTemplateTemp = templatesListTemp.find((t:Plantilla) => t.idPlantilla === currentTemplate.idPlantilla);
          setSelectedTemplate(selectedTemplateTemp);
        }          
 
        setLoading(false);
      }
      else {
        enqueueSnackbar("Error al obtener las plantillas.", {
          variant: "error",
        });
      }
      
    } catch (error) {
      enqueueSnackbar("Error al obtener las plantillas. " + error, {
        variant: "error",
      });
      setLoading(false);
    } */

  }, [enqueueSnackbar]);

  useEffect(() => {

    //fetchTemplates(selectedWorkflow.idProceso, undefined);

  }, [selectedWorkflow, fetchTemplates]);

  return (
    <>
      <DialogTitle>{"Comparar Plantillas"}</DialogTitle>
      <DialogContent>
        <DialogContentText>Visualización comparativa de la versión actual vs la versión porpuesta</DialogContentText>
        <Box sx={{ my: 3 }}>
          <Typography variant="subtitle1"></Typography>
          <Paper
            variant="outlined"
            sx={{ my: { xs: 2, md: 2 }, p: { xs: 2, md: 3 } }}
          >
            <Grid container spacing={3}>
              <Grid item xs={12} md={6}>
                <Typography variant="h6" gutterBottom>Versión Actual</Typography>
                <Alert severity="info" sx={{ mb: 2 }}>
                  <div>
                    <strong>Secciones:</strong>
                  </div>
                  <div style={{ marginTop: "8px" }}>
                    Una sección es utilizada para agrupar los campos de la
                    plantilla de acuerdo a su contexto. Por defecto una
                    plantilla contendrá la sección de{" "}
                    <strong>Datos Generales</strong>, pero será posible agregar una o más secciones.
                  </div>
                </Alert>
                {selectedTemplate && (
                  <SectionsListViewer
                    selectedTemplate={originalTemplate}
                  />
                )}
              </Grid>

              <Grid item xs={12} md={6}>
                <Typography variant="h6" gutterBottom>Versión Propuesta</Typography>
                <Alert severity="info" sx={{ mb: 2 }}>
                  <div>
                    <strong>Secciones:</strong>
                  </div>
                  <div style={{ marginTop: "8px" }}>
                    Una sección es utilizada para agrupar los campos de la
                    plantilla de acuerdo a su contexto. Por defecto una
                    plantilla contendrá la sección de{" "}
                    <strong>Datos Generales</strong>, pero será posible agregar una o más secciones.
                  </div>
                </Alert>
                {selectedTemplate && (
                  <SectionsListViewer
                    selectedTemplate={selectedTemplate}
                  />
                )}
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
