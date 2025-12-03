import React, { useCallback, useEffect, useState } from 'react'

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
  TextField,
  Alert,
  Autocomplete,
  IconButton,
  Stack,
  Dialog,
  Tooltip
} from '@mui/material';
import AddIcon from '@mui/icons-material/Add'
import EditIcon from '@mui/icons-material/Edit'
import PreviewIcon from '@mui/icons-material/Preview';
import DeleteIcon from '@mui/icons-material/Delete';
import UndoIcon from '@mui/icons-material/Undo';
import { useTranslation } from 'react-i18next';

// Toastr Dependencies
import { useSnackbar } from 'notistack';
import { Proceso } from '../types/Proceso';
import { templateService } from '../services/settings/templateService';
import TemplateAddEditDialog from './TemplateAddEditDialog';
import { HistoricoPlantillas, Plantilla } from '../types/Plantilla';
import AlertDialog from '../components/AlertDialog';
import TemplatePreviewDialog from './TemplatePreviewDialog';
import SectionsList from '../components/SectionsList';
import CompareIcon from '@mui/icons-material/Compare';
import PublishIcon from '@mui/icons-material/Publish';
import TemplateCompareDialog from './TemplateCompareDialog';

// Dialog parameters Type
type DialogProps = {
  selectedWorkflow: Proceso,
  onClose: (refreshWorkflowList: boolean) => void
}

// Other global variables

export default function WorkflowTemplatesDialog({ selectedWorkflow, onClose }: DialogProps) {

  // Local constants or varialbes    
  const [t] = useTranslation();
  const { enqueueSnackbar } = useSnackbar();

  const [loading, setLoading] = useState<boolean>(false);
  const [templatesList, setTemplatesList] = useState([] as any);
  const [templatesNamesList, setTemplatesNamesList] = useState<any[]>([]);
  const [selectedTemplate, setSelectedTemplate] = useState<Plantilla | undefined>(undefined);
  const [openTemplateAddEditDialog, setOpenTemplateAddEditDialog] = useState<boolean>(false);
  const [openTemplatePreviewDialog, setOpenTemplatePreviewDialog] = useState<boolean>(false);
  const [openTemplateDeleteDialog, setOpenTemplateDeleteDialog] = useState<boolean>(false);
  const [openTemplateRevertDialog, setOpenTemplateRevertDialog] = useState<boolean>(false);
  const [templateNamesListValue, setTemplateNamesListValue] = React.useState<any>(null);
  const [openPublishTemplateDialog, setOpenPublishTemplateDialog] = useState<boolean>(false);
  const [openTemplateCompareDialog, setOpenTemplateCompareDialog] = useState<boolean>(false);
  const [originalTemplate, setOriginalTemplate] = useState<Plantilla | undefined>(undefined);
  const [_workflowId, setWorkflowId] = useState<number>(0);

  // Fetch functions
  const fetchTemplates = useCallback(async (workflowId: number, currentTemplate: Plantilla | undefined) => {

    try {
      const response = await templateService.getAll(workflowId);
      setWorkflowId(workflowId);
      if (response.statusText === "OK") {
        const templatesListTemp = [...response.data];

        setTemplatesList(templatesListTemp);
        setTemplatesNamesList(templatesListTemp.map((t: any) => {
          return { label: t.nombre + (t.activa ? ' (Activa)' : ''), id: t.idPlantilla };
        }));

        if (currentTemplate) {
          const selectedTemplateTemp = templatesListTemp.find((t: Plantilla) => t.idPlantilla === currentTemplate.idPlantilla);
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
    }

  }, [enqueueSnackbar]);

  // Template Add - Edit
  const handleCloseTemplateAddEditDialog = () => {
    setOpenTemplateAddEditDialog(false);
  }

  const handleCloseTemplateAddEditDialogFromAction = async (actionResult: boolean = false) => {
    if (actionResult) {
      await fetchTemplates(selectedWorkflow.idProceso, selectedTemplate);
    }
    setOpenTemplateAddEditDialog(false);
  }

  const handleOpenTemplateAddEditDialog = async (template: any) => {

    if (!template) {
      setSelectedTemplate(undefined);
    }

    setOpenTemplateAddEditDialog(true);
  }

  // Template Preview
  const handleOpenTemplatePreviewDialog = async (template: any) => {

    if (!template) {
      setSelectedTemplate(undefined);
    }

    setOpenTemplatePreviewDialog(true);
  }

  const handleCloseTemplatePreviewDialog = () => {
    setOpenTemplatePreviewDialog(false);
  }

  // Template Delete
  const handleOpenTemplateDeleteDialog = async (user: any) => {
    setOpenTemplateDeleteDialog(true);
  }

  const handleCloseTemplateDeleteDialog = () => {
    setOpenTemplateDeleteDialog(false);
  }

  const handleCloseTemplateDeleteDialogFromAction = async (actionResult: boolean = false) => {
    if (actionResult) {
      if (selectedTemplate) {
        await deleteSelectedTemplate(selectedTemplate.idProceso, selectedTemplate.idPlantilla);
        await fetchTemplates(selectedWorkflow.idProceso, selectedTemplate);
      }
      else {
        enqueueSnackbar("Debe seleccionar una plantilla para eliminar.");
      }
    }
    setOpenTemplateDeleteDialog(false);
  }

  const deleteSelectedTemplate = async (workflowId: number, templateId: number) => {
    setLoading(true);

    try {

      const response = await templateService.delete(workflowId, templateId);

      if (response.statusText === "OK") {
        setSelectedTemplate(undefined);
        enqueueSnackbar("Plantilla eliminada.", { variant: "success" });
      } else {
        enqueueSnackbar("Ocurrió un error al eliminar la plantilla.", {
          variant: "error",
        });
      }

    } catch (error: any) {
      enqueueSnackbar(
        "Ocurrió un error al eliminar la plantilla.. Detalles: " +
        error.message,
        { variant: "error" }
      );

    }

    setLoading(false);
  };

    // Template Revert Changes
    const handleOpenTemplateRevertDialog = async (user: any) => {
      setOpenTemplateRevertDialog(true);
    }
  
    const handleCloseTemplateRevertDialog = () => {
      setOpenTemplateRevertDialog(false);
    }
  
    const handleCloseTemplateRevertDialogFromAction = async (actionResult: boolean = false) => {
      if (actionResult) {
        if (selectedTemplate) {
          await revertSelectedTemplate(true);
          await fetchTemplates(selectedWorkflow.idProceso, selectedTemplate);
        }
        else {
          enqueueSnackbar("Debe seleccionar una plantilla para revertir.");
        }
      }
      setOpenTemplateRevertDialog(false);
    }
  
    const revertSelectedTemplate = async (actionResult: boolean = false) => {
    
      if (actionResult) {

        if (selectedTemplate) {
  
          setLoading(true);
  
          try {
  
            const templateHistory: HistoricoPlantillas = {
              idEntidad: 0,
              idProceso: parseInt(selectedTemplate.idProceso.toString()),
              idPlantilla: parseInt(selectedTemplate.idPlantilla.toString()),
              version: selectedTemplate.versionPropuesta,
              fechaPublicacion: new Date()
            };
  
            const response = await templateService.revert(templateHistory);
            if (response.statusText === "OK") {
              enqueueSnackbar("Plantilla revertida.", { variant: "success" });
              fetchTemplates(selectedWorkflow.idProceso, selectedTemplate);
            }
            else {
              enqueueSnackbar("Ocurrió un error al revertir los cambios a la plantilla.", {
                variant: "error",
              });
            }
          }
          catch (error: any) {
            enqueueSnackbar(
              "Ocurrió un error al revertir los cambios a la plantilla. Detalles: " +
              error.message,
              { variant: "error" }
            );
          }
          finally {
            setLoading(false);
          }
  
        }
        else {
          enqueueSnackbar("Debe seleccionar una plantilla para revertir los cambios.");
        }
      }

    }

  // Other functions
  const handleSelectedTemplateChange = async (selectedValue: any) => {

    if (selectedValue) {
      setTemplateNamesListValue(selectedValue);
      setSelectedTemplate(templatesList.find((t: Plantilla) => t.idPlantilla === selectedValue.id));
    }
    else {
      setSelectedTemplate(undefined);
    }

  }

  const handleClosePublishTemplateDialog = () => {
    setOpenPublishTemplateDialog(false);
  }

  const handleClosePublishTemplateDialogFromAction = async (actionResult: boolean = false) => {

    if (actionResult) {

      console.log("Publish template", selectedTemplate);

      if (selectedTemplate) {

        setLoading(true);

        try {

          const templateHistory: HistoricoPlantillas = {
            idEntidad: 0,
            idProceso: parseInt(selectedTemplate.idProceso.toString()),
            idPlantilla: parseInt(selectedTemplate.idPlantilla.toString()),
            version: selectedTemplate.versionPropuesta,
            fechaPublicacion: new Date()
          };

          const response = await templateService.publishTemplate(templateHistory);
          if (response.statusText === "OK") {
            enqueueSnackbar("Plantilla publicada.", { variant: "success" });
            fetchTemplates(selectedWorkflow.idProceso, selectedTemplate);
          }
          else {
            enqueueSnackbar("Ocurrió un error al publicar la plantilla.", {
              variant: "error",
            });
          }
        }
        catch (error: any) {
          enqueueSnackbar(
            "Ocurrió un error al publicar la plantilla. Detalles: " +
            error.message,
            { variant: "error" }
          );
        }
        finally {
          setLoading(false);
        }

      }
      else {
        enqueueSnackbar("Debe seleccionar una plantilla para eliminar.");
      }
    }

    setOpenPublishTemplateDialog(false);

  }

  // Template Compare Dialog
  const handleCloseTemplateCompareDialog = () => {
    setOpenTemplateCompareDialog(false);
  }

  const handleOpenTemplateCompareDialog = () => {
    setOpenTemplateCompareDialog(true);
  }

  useEffect(() => {

    fetchTemplates(selectedWorkflow.idProceso, undefined);

  }, [selectedWorkflow, fetchTemplates]);

  const fetchOriginalTemplate = async (selectedValue: any) => {

    if (selectedValue) {
      const response = await templateService.getSpecificTamplate(_workflowId, selectedValue.id);
      if (response.statusText === "OK") {
        const actualTemplateTemp = [...response.data];
        console.log("actualTemplateTemp ", actualTemplateTemp);

        const actualTemplate = actualTemplateTemp.find((t: Plantilla) => t.idPlantilla === selectedValue.id);
        setOriginalTemplate(actualTemplate);
      }

    }
  }

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
            {selectedWorkflow.nombre + " - Plantillas"}
          </div>
        </div>
      </DialogTitle>
      <DialogContent>
        <DialogContentText>
          {selectedWorkflow.descripcion}
        </DialogContentText>
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
                    <span style={{ fontWeight: "bold" }}>Plantilla:</span>
                  </div>
                  <div style={{ marginTop: "8px" }}>
                    La plantilla de un proceso contiene secciones y campos
                    que deben llenarse cada vez que un expediente
                    perteneciente dicho proceso ingresa. Un proceso puede
                    tener múltiples plantillas configuradas, pero{" "}
                    <span style={{ fontWeight: "bold" }}>
                      sólo una plantilla estará activa en todo momento.
                    </span>
                  </div>
                  <div style={{ marginTop: "8px" }}>
                    Seleccione la plantilla de su interés para poder
                    visualizar o modificar las secciones y campos
                    configurados.
                  </div>
                </Alert>
              </Grid>
              <Grid item xs={12}>
                <Stack
                  direction="row"
                  // alignItems="center"
                  sx={{
                    // justifyContent: "center",
                    alignItems: "center",
                  }}
                >
                  <Autocomplete
                    sx={{ minWidth: "50%" }}
                    disablePortal
                    id="selectedTemplate"
                    options={templatesNamesList}
                    isOptionEqualToValue={(option: any, value: any) =>
                      option.id === value.id
                    }
                    //defaultValue={null}
                    getOptionLabel={(option) => option.label || ''}
                    value={templateNamesListValue}
                    onChange={(event: any, newValue: any | null) => {
                      fetchOriginalTemplate(newValue);
                      handleSelectedTemplateChange(newValue);
                    }}
                    renderInput={(params) => (
                      <TextField
                        {...params}
                        label="Plantilla"
                        variant="standard"
                      />
                    )}
                  />
                  <IconButton onClick={() => handleOpenTemplateAddEditDialog(null)}>
                    <Tooltip title="Crear nueva plantilla" arrow placement="top-start">
                      <AddIcon />
                    </Tooltip>
                  </IconButton>
                  <IconButton onClick={() => handleOpenTemplateAddEditDialog(selectedTemplate)} disabled={!selectedTemplate}>
                    <Tooltip title="Editar plantilla" arrow placement="top-start">
                      <EditIcon />
                    </Tooltip>
                  </IconButton>
                  <IconButton onClick={() => handleOpenTemplatePreviewDialog(selectedTemplate)} disabled={!selectedTemplate}>
                    <Tooltip title="Previsualizar plantilla" arrow placement="top-start">
                      <PreviewIcon />
                    </Tooltip>
                  </IconButton>
                  <IconButton onClick={() => handleOpenTemplateRevertDialog(selectedTemplate)} disabled={!selectedTemplate}>
                    <Tooltip title="Revertir cambios a la plantilla" arrow placement="top-start">
                      <UndoIcon />
                    </Tooltip>
                  </IconButton>
                  <IconButton onClick={() => handleOpenTemplateDeleteDialog(selectedTemplate)} disabled={!selectedTemplate} >
                    <Tooltip title="Eliminar plantilla" arrow placement="top-start">
                      <DeleteIcon />
                    </Tooltip>
                  </IconButton>
                </Stack>
              </Grid>
              <Grid item xs={12}>
                <Alert severity="info">
                  <div>
                    <span style={{ fontWeight: "bold" }}>Secciones:</span>
                  </div>
                  <div style={{ marginTop: "8px" }}>
                    Una sección es utilizada para agrupar los campos de la
                    plantilla de acuerdo a su contexto. Por defecto una
                    plantilla contendrá la sección de{" "}
                    <span style={{ fontWeight: "bold" }}>
                      "Datos Generales"
                    </span>
                    , pero será posible agregar una o más secciones.
                  </div>
                </Alert>
              </Grid>
              <Grid item xs={12}>
                {
                  selectedTemplate ? (
                    <SectionsList
                      selectedTemplate={selectedTemplate}
                      callbackAction={() => { fetchTemplates(selectedWorkflow.idProceso, selectedTemplate) }}
                    ></SectionsList>
                  ) :
                    (
                      <></>
                    )
                }

              </Grid>
            </Grid>
          </Paper>
        </Box>
      </DialogContent>
      <DialogActions>
        {
          selectedTemplate && selectedTemplate.version !== selectedTemplate.versionPropuesta ? (
            <Box sx={{ flexGrow: 1, display: 'flex', justifyContent: 'flex-start' }}>
              <Button
                variant="outlined"
                color="secondary"
                disableElevation
                startIcon={<CompareIcon />}
                onClick={() => {
                  if (!selectedTemplate) {
                    enqueueSnackbar("Debe seleccionar una plantilla para comparar.", { variant: "warning" });
                    return;
                  }
                  handleOpenTemplateCompareDialog();
                }}
              >
                Comparar Plantillas
              </Button>

              <Button
                variant="outlined"
                color="primary"
                disableElevation
                startIcon={<PublishIcon />}
                onClick={() => {
                  if (!selectedTemplate) {
                    enqueueSnackbar("Debe seleccionar una plantilla para publicar.", { variant: "warning" });
                    return;
                  }
                  setOpenPublishTemplateDialog(true);
                }}
                sx={{ ml: 2 }}
              >
                Confirmar y publicar cambios
              </Button>
            </Box>
          ) :
            (
              <></>
            )
        }

        <Box sx={{ display: 'flex', justifyContent: 'flex-end' }}>
          <Button
            variant="outlined"
            onClick={() => {
              onClose(false);
            }}
          >
            Cerrar
          </Button>
        </Box>

      </DialogActions>

      <Dialog
        open={openTemplateAddEditDialog}
        onClose={handleCloseTemplateAddEditDialog}
        maxWidth={"sm"}
      >
        <TemplateAddEditDialog
          selectedTemplate={selectedTemplate}
          selectedWorkflow={selectedWorkflow}
          mode={!selectedTemplate ? "add" : "edit"}
          onClose={handleCloseTemplateAddEditDialogFromAction}
        />
      </Dialog>

      <Dialog
        open={openTemplatePreviewDialog}
        onClose={handleCloseTemplatePreviewDialog}
        maxWidth={"lg"}
      >
        <TemplatePreviewDialog
          selectedTemplate={selectedTemplate}
          selectedWorkflow={selectedWorkflow}
          mode={!selectedTemplate ? "add" : "edit"}
          onClose={handleCloseTemplatePreviewDialog}
        />
      </Dialog>

      <Dialog
        open={openTemplateDeleteDialog}
        onClose={handleCloseTemplateDeleteDialog}
        maxWidth={"sm"}
      >
        <AlertDialog
          color={"error"}
          title={"Eliminar Plantilla"}
          message={
            <>
              ¿Está seguro que desea eliminar la plantilla: <b>{selectedTemplate?.nombre}</b>? Esta acción no se puede deshacer.
            </>
          }
          onClose={handleCloseTemplateDeleteDialogFromAction}
        />
      </Dialog>

      <Dialog
        open={openPublishTemplateDialog}
        onClose={handleClosePublishTemplateDialog}
        maxWidth={"sm"}
      >
        <AlertDialog
          color={"primary"}
          title={"Publicar Plantilla"}
          loading={loading}
          loadingMessage={"Publicando plantilla..."}
          message={
            <>
              ¿Está seguro que desea publicar la nueva versión de la plantilla: <b>{selectedTemplate?.nombre}</b>? Una vez publicada la nueva versión no podrá volver a la versión anterior.
            </>
          }
          onClose={handleClosePublishTemplateDialogFromAction}
        />
      </Dialog>

      <Dialog
        open={openTemplateCompareDialog}
        onClose={handleCloseTemplateCompareDialog}
        maxWidth={"xl"}
      >
        <TemplateCompareDialog
          originalTemplate={originalTemplate}
          selectedTemplate={selectedTemplate}
          selectedWorkflow={selectedWorkflow}
          onClose={handleCloseTemplateCompareDialog}
        />
      </Dialog>

      <Dialog
        open={openTemplateRevertDialog}
        onClose={handleCloseTemplateRevertDialog}
        maxWidth={"sm"}
      >
        <AlertDialog          
          title={"Revertir Cambios a la Plantilla"}
          message={
            <>
              ¿Está seguro que desea revertir los cambios a la plantilla: <b>{selectedTemplate?.nombre}</b>? Esta acción no se puede deshacer.
            </>
          }
          onClose={handleCloseTemplateRevertDialogFromAction}
        />
      </Dialog>
    </>
  );
}
