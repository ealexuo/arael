import React, { useCallback, useEffect, useState } from 'react'
import Page from '../../components/Page'
import { administrativeUnitsService } from '../../services/settings/administrativeUnitsService';
import { useSnackbar } from 'notistack';
import { workflowService } from '../../services/settings/workflowService';
import { randomValuesService } from '../../services/utilities/randomValuesService';
import { Proceso } from '../../types/Proceso';
import { useNavigate } from 'react-router-dom';
import { useParams } from 'react-router-dom';

import { fileService } from '../../services/files/fileService';
import { Expediente } from '../../types/Expediente';
import { Accordion, AccordionDetails, AccordionSummary, Box, Button, Card, CardActionArea, CardContent, Grid, List, ListItem, ListItemText, Paper, Tab, Tabs, TextField, Typography } from '@mui/material';

import ExpandMoreIcon from '@mui/icons-material/ExpandMore';
import { originService } from '../../services/settings/originService';
import { Origen } from '../../types/Origen';
import { stringConverterService } from '../../services/utilities/stringConverterService';
import FileData from '../../components/FileData';

const searchText: string = '';
const rowsPerPage: number = 500;
const currentPage: number = 0;

const emptyWorkflowObject: Proceso = {
  idEntidad: 0,
  idProceso: -1,
  nombre: '',
  descripcion: '',
  color: randomValuesService.getHexadecimal(),
  idTipoProceso: 0,
  tipoProceso: 'Estándar',
  idUnidadAdministrativa: 0,
  unidadAdministrativa: '',
  siglasUA: '',
  estado: false,
  expedientesActivos: 0,
  expedientesFinalizados: 0,
  totalExpedientes: 0
}
const label = { inputProps: { 'aria-label': 'Checkbox demo' } };

export default function FileDetails() {

  const { fileId } = useParams();
  const [file, setFile] = useState<Expediente | null>(null);
  const [workflow, setWorkflow] = useState<Proceso | null>(null);
  const [origin, setOrigin] = useState<Origen | null>(null);

  const { enqueueSnackbar } = useSnackbar();
  const [loading, setLoading] = useState<boolean>(true);
  const [value, setValue] = React.useState(0);

  const handleChange = (event: React.SyntheticEvent, newValue: number) => {
    setValue(newValue);
  };

  
  interface TabPanelProps {
    children?: React.ReactNode;
    index: number;
    value: number;
  }

  interface TabPanelProps {
    children?: React.ReactNode;
    index: number;
    value: number;
  }

  function CustomTabPanel(props: TabPanelProps) {
    const { children, value, index, ...other } = props;

    return (
      <div
        role="tabpanel"
        hidden={value !== index}
        id={`simple-tabpanel-${index}`}
        aria-labelledby={`simple-tab-${index}`}
        {...other}
      >
        {value === index && <Box sx={{ p: 3 }}>{children}</Box>}
      </div>
    );
  }

  function a11yProps(index: number) {
    return {
      id: `simple-tab-${index}`,
      'aria-controls': `simple-tabpanel-${index}`,
    };
  }



  // const [openWorkflowAddEditDialog, setOpenWorkflowAddEditDialog] = useState<boolean>(false);
  // const [openWorkflowTemplatesDialog, setOpenWorkflowTemplatesDialog] = useState<boolean>(false);
  // const [openWorkflowPhasesDialog, setOpenWorkflowPhasesDialog] = useState<any>(null);
  // const [openWorkflowRequirementsDialog, setOpenWorkflowRequirementsDialog] = useState<any>(null);
  // const [openWorkflowDeleteDialog, setOpenWorkflowDeleteDialog] = useState<boolean>(false);
  // const [selectionMode, setselectionMode] = useState<string>("multiple");
  // const [checkboxesChecked, setCheckboxesChecked] = useState<boolean>(false);
  // const [selectedFiles, setSelectedFiles] = useState<number[]>([]);
  // const [selectedProcessType, setSelectedProcessType] = useState<number | null>(null);
  // const [selectedWorkflow, setSelectedWorkflow] = useState<any>(null);
  // const [administrativeUnitslist, setAdministrativeUnitslist] = useState<any>(null);
  // const [workflowList, setWorkflowList] = useState<Proceso[]>([]);
  // const [filesList, setFilesList] = useState<Expediente[]>([]);
  // const [filesTotal, setFilesTotal] = useState<number>(0);

  

  /** Fetch Data Section */

  const fetchData = useCallback(async () => {
    try {

      setLoading(true);     

      const fileResponse = await fileService.get(fileId ? parseInt(fileId) : 0);

      if (fileResponse.statusText === 'OK') {
        setFile(fileResponse.data);

        const workflowResponse = await workflowService.get(fileResponse.data.idProceso);
        const originResponse = await originService.get(fileResponse.data.idOrigen);

        if(workflowResponse.statusText === 'OK' && originResponse.statusText === 'OK') {
          setWorkflow(workflowResponse.data);
          setOrigin(originResponse.data);
        }
        else {
          enqueueSnackbar('Ocurrió un error al obtener información del proceso o del origen.', { variant: 'error' });
        }
      }
      else {
        enqueueSnackbar('Ocurrió un error al obtener información expediente.', { variant: 'error' });
      }
    }
    catch (error: any) {
      enqueueSnackbar('Ocurrió un error al obtener informaciión del expediente. Detalles: ' + error.message, { variant: 'error' });
    }
    finally {
      setLoading(false);
    }
  }, [enqueueSnackbar]);

  // const fetchWorkflows = useCallback(async (initialPage: number, itemsPerPage: number, searchString: string) => {
  //   try {
  //     setLoading(true);
  //     const response = await workflowService.getAll(initialPage + 1, itemsPerPage, searchString);

  //     if (response.statusText === 'OK') {
  //       setWorkflowList(response.data);
  //     }
  //     else {
  //       enqueueSnackbar('Ocurrió un error al obtener la lista de procesos.', { variant: 'error' });
  //     }
  //   }
  //   catch (error: any) {
  //     enqueueSnackbar('Ocurrió un error al obtener la lista de procesos. Detalles: ' + error.message, { variant: 'error' });
  //   }
  //   finally {
  //     setLoading(false);
  //   }
  // }, [enqueueSnackbar]);

  // const deleteSelectedWorkflow = async (workflowId: number) => {

  //   setLoading(true);

  //   try {
  //     const response = await workflowService.delete(workflowId);

  //     if (response.statusText === "OK") {
  //       enqueueSnackbar('Proceso eliminado.', { variant: "success" });
  //     } else {
  //       enqueueSnackbar('Ocurrió un Error al eliminar el proceso.', { variant: "error" });
  //     }
  //   } catch (error: any) {
  //     enqueueSnackbar('Ocurrió un Error al eliminar el proceso. Detalles: ' + error.message, { variant: "error" });
  //   }
  //   finally {
  //     setLoading(false);
  //   }

  //}

  // const fetchAdministrativeUnits = useCallback(async () => {

  //   setLoading(true);
  //   try {
  //     const response = await administrativeUnitsService.getAll();
  //     if (response.statusText === 'OK') {
  //       const administrativeUnits = response.data;

  //       setAdministrativeUnitslist(administrativeUnits.map((ua: any) => {
  //         return {
  //           id: ua.idUnidadAdministrativa, label: ua.nombre
  //         }
  //       }));
  //     }
  //     else {
  //       enqueueSnackbar('Ocurrió un Error al obtener las unidades administrativas.', { variant: 'error' });
  //     }
  //   }
  //   catch (error: any) {
  //     enqueueSnackbar('Ocurrió un Error al obtener las unidades administrativas. Detalles: ' + error.message, { variant: 'error' });
  //   }
  //   finally {
  //     setLoading(false);
  //   }

  //   return null;
  // }, [enqueueSnackbar]);

  /** Handle Functions Section */

  // // Workflow Add/Edit dialog
  // const handleOpenWorkflowAddEditDialog = () => {
  //   setSelectedWorkflow(emptyWorkflowObject);
  //   setOpenWorkflowAddEditDialog(true);
  // }

  // const handleCloseUserAddEditDialog = () => {
  //   setOpenWorkflowAddEditDialog(false);
  // }

  // const handleCloseUserAddEditDialogFromAction = (refreshUsersList: boolean = false) => {
  //   if (refreshUsersList) {
  //     fetchWorkflows(currentPage, rowsPerPage, searchText);
  //   }
  //   setOpenWorkflowAddEditDialog(false);
  // }

  // const handleSelectionMode = async (mode: string) => {
  //   setselectionMode(mode);
  //   setCheckboxesChecked(false);
  //   setSelectedFiles([]);
  //   setSelectedProcessType(null);

  //   if (mode === "multiple") {
  //     enqueueSnackbar("Ahora podrás seleccionar expedientes de diferentes tipos de proceso para vincularlos", { variant: "info" });
  //   } else {
  //     enqueueSnackbar("Ahora únicamente podrás seleccionar expedientes de un mismo tipo para operaciones masivas", { variant: "info" });
  //   }
  // }

  // const handleSelectedWorkflowEdit = async (workflow: any) => {
  //   //setOpenWorkflowAddEditDialog(true);
  // }

  // // User Disable dialog
  // const handleOpenWorkflowTemplatesDialog = async (workflow: any) => {
  //   setSelectedWorkflow(workflow);
  //   setOpenWorkflowTemplatesDialog(true);
  // }

  // const handleCloseWorkflowTemplatesDialog = () => {
  //   setOpenWorkflowTemplatesDialog(false);
  // }

  // const handleCloseWorkflowTemplatesDialogFromAction = (refreshWorkflowsList: boolean = false) => {
  //   if (refreshWorkflowsList) {
  //     fetchWorkflows(currentPage, rowsPerPage, searchText);
  //   }
  //   setOpenWorkflowTemplatesDialog(false);
  // }

  // // Worflow Delete Alert dialog
  // const handleOpenWorkflowDeleteDialog = async (workflow: any) => {
  //   setSelectedWorkflow(workflow);
  //   setOpenWorkflowDeleteDialog(true);
  // }

  // const handleCloseWorkflowDeleteDialog = () => {
  //   setOpenWorkflowDeleteDialog(false);
  // }

  // const handleCloseWorkflowDeleteDialogFromAction = async (actionResult: boolean = false) => {
  //   if (actionResult) {
  //     await deleteSelectedWorkflow(selectedWorkflow.idProceso);
  //     await fetchWorkflows(currentPage, rowsPerPage, searchText);
  //   }
  //   setOpenWorkflowDeleteDialog(false);
  // }

  // // Phases and transitions dialog
  // const handleWorkflowPhasesDialog = async (workflow: any) => {
  //   setSelectedWorkflow(workflow);
  //   setOpenWorkflowPhasesDialog(true);
  // }

  // const handleCloseWorkflowPhasesDialog = () => {
  //   setOpenWorkflowPhasesDialog(false);
  // }

  // const handleCloseWorkflowPhasesDialogFromAction = (refreshWorkflowsList: boolean = false) => {
  //   if (refreshWorkflowsList) {
  //     fetchWorkflows(currentPage, rowsPerPage, searchText);
  //   }
  //   setOpenWorkflowPhasesDialog(false);
  // }

  // // Creation requirements
  // const handleWorkflowRequirementsDialog = async (workflow: any) => {
  //   setSelectedWorkflow(workflow);
  //   setOpenWorkflowRequirementsDialog(true);
  // }

  // const handleCloseWorkflowRequirementsDialog = () => {
  //   setOpenWorkflowRequirementsDialog(false);
  // }

  // const handleCloseWorkflowRequirementsDialogFromAction = (refreshWorkflowsList: boolean = false) => {
  //   if (refreshWorkflowsList) {
  //     fetchWorkflows(currentPage, rowsPerPage, searchText);
  //   }
  //   setOpenWorkflowRequirementsDialog(false);
  // }

  // const handleSelectAll = (event: React.ChangeEvent<HTMLInputElement>) => {
  //   if (event.target.checked) {
  //     const allFileIds = filesList.map(file => file.idExpediente);
  //     setSelectedFiles(allFileIds);
  //   } else {
  //     setSelectedFiles([]);
  //   }
  //   setCheckboxesChecked(event.target.checked);
  // };

  // const handleSelectOne = (fileId: number, processId: number) => {
  //   if (selectionMode === "tipos") {
  //     setSelectedFiles(prev => {
  //       if (prev.includes(fileId)) {
  //         const newSelection = prev.filter(id => id !== fileId);
  //         // Si no quedan selecciones, reiniciamos el tipo de proceso
  //         if (newSelection.length === 0) {
  //           setSelectedProcessType(null);
  //         }
  //         return newSelection;
  //       } else {
  //         // Primero agregamos el archivo a la selección
  //         const newSelection = [...prev, fileId];
  //         // Luego actualizamos el tipo de proceso si es la primera selección
  //         if (prev.length === 0) {
  //           setSelectedProcessType(processId);
  //         }
  //         return newSelection;
  //       }
  //     });
  //   } else {
  //     // Lógica existente para modo múltiple
  //     setSelectedFiles(prev => {
  //       if (prev.includes(fileId)) {
  //         return prev.filter(id => id !== fileId);
  //       } else {
  //         return [...prev, fileId];
  //       }
  //     });
  //   }
  // };

  
  useEffect(() => {

    fetchData();

  }, [fetchData]);

  /** Return Section */
  return (
    <>
      <Page title="Detalle del Expediente">
        <Grid container spacing={2}>
          <Grid item xs={12}>
            <Paper elevation={2} style={{ backgroundColor: '#F0F4F8' }}> 
              <List>
                <ListItem style={{ display: 'flex', alignItems: 'center' }}>
                  <div
                    style={{
                      display: "inline-block",
                      width: "2mm",
                      height: "13mm",
                      backgroundColor: workflow ? workflow.color : '#E0E0E0',
                      marginRight: 12,
                    }}
                  ></div>
                  <ListItemText
                    primary={`Expediente: ${file ? stringConverterService.formatFieldId(file.idExpediente) : 'Cargando...'}`}
                    secondary={workflow ? `Proceso: ${workflow.nombre}` : 'Cargando...'}
                  />
                </ListItem>
              </List>
            </Paper>
          </Grid>
          <Grid item xs={12}>
            <Paper elevation={2}>
              <Accordion>
                <AccordionSummary
                  expandIcon={<ExpandMoreIcon />}
                  aria-controls="panel1-content"
                  id="panel1-header"
                >
                  <Typography component="span">Datos Generales del Expediente</Typography>
                </AccordionSummary>
                <Box sx={{ px: 2 }}>
                  <Box
                    sx={{
                      borderBottom: 1,
                      borderColor: 'divider',
                      mb: 1,
                    }}
                  />
                </Box>
                <AccordionDetails>
                  <Grid container spacing={3}>
                    <Grid item xs={12} sm={6}>
                        <TextField
                            label="Proceso"
                            fullWidth
                            variant="standard"
                            value={workflow ? workflow.nombre : 'Cargando...'}
                            disabled={true}                        
                        />
                    </Grid>
                    <Grid item xs={12} sm={6}>
                        <TextField
                            label="Origen"
                            fullWidth
                            variant="standard"
                            value={origin ? origin.nombre : 'Cargando...'}
                            disabled={true}                        
                        />
                    </Grid>
                    <Grid item xs={12}>
                        <TextField
                            label="Emisor"
                            fullWidth
                            variant="standard"
                            value={file ? file.emisor : 'Cargando...'}
                            disabled={true}                        
                        />
                    </Grid>
                    <Grid item xs={12}>
                        <TextField
                            label="Descripción"
                            fullWidth
                            variant="standard"
                            value={file ? file.descripcion : 'Cargando...'}
                            disabled={true}                        
                        />
                    </Grid>
                  </Grid>
                </AccordionDetails>
              </Accordion>
            </Paper>
          </Grid>
          <Grid item xs={12}>
            <Paper elevation={2}> 
              <Box sx={{ width: '100%', p: 2 }}>
                <Box sx={{ borderBottom: 1, borderColor: 'divider' }}>
                  <Tabs value={value} onChange={handleChange} aria-label="tabs">
                    <Tab label="Datos" {...a11yProps(0)} />
                    <Tab label="Documentos Adjuntos" {...a11yProps(1)} />
                    <Tab label="Requisitos de Gestión" {...a11yProps(2)} />
                    <Tab label="Histórico" {...a11yProps(3)} />
                  </Tabs>
                </Box>
                <CustomTabPanel value={value} index={0}>
                  <FileData workflow={workflow} file={file} />
                </CustomTabPanel>
                <CustomTabPanel value={value} index={1}>
                  Contenido - Documentos Adjuntos
                </CustomTabPanel>
                <CustomTabPanel value={value} index={2}>
                  Contenido - Requisitos de Gestión
                </CustomTabPanel>
                <CustomTabPanel value={value} index={3}>
                  Contenido - Histórico
                </CustomTabPanel>
              </Box>
            </Paper>
          </Grid>
          <Grid item xs={12}>
            <Paper elevation={2}> 
                <Box sx={{ width: '100%', p: 2, display: 'flex', gap: 2, justifyContent: 'center' }}>
                  <Button 
                    variant="outlined"                    
                  >
                    Regresar
                  </Button>
                  <Button
                    variant="contained"
                    disableElevation
                  >
                    Imprimir
                  </Button>
                </Box>
            </Paper>
          </Grid>
        </Grid>
      </Page>

      {/* <Dialog
        open={openWorkflowAddEditDialog}
        onClose={handleCloseUserAddEditDialog}
        maxWidth={"lg"}
      >
        <WorkflowAddEditDialog
          mode={selectedWorkflow && selectedWorkflow.idProceso > -1 ? 'edit' : 'add'}
          selectedWorkflow={selectedWorkflow}
          administrativeUnitsList={administrativeUnitslist}
          onClose={handleCloseUserAddEditDialogFromAction}
        />
      </Dialog> */}      

      {/* <Dialog
        open={openWorkflowDeleteDialog}
        onClose={handleCloseWorkflowDeleteDialog}
        maxWidth={"sm"}
      >
        <AlertDialog
          color={'error'}
          title={'Eliminar Proceso'}
          message={
            <>
              ¿Está seguro que desea eliminar el proceso: <b>{selectedWorkflow?.nombre}</b>? Esta acción no se puede deshacer.
            </>
          }
          onClose={handleCloseWorkflowDeleteDialogFromAction}
        />
      </Dialog> */}

    </>
  );
}


