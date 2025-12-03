import React, { useCallback, useEffect, useState } from 'react'
import Page from '../../components/Page'
import { administrativeUnitsService } from '../../services/settings/administrativeUnitsService';
import AddIcon from '@mui/icons-material/Add'
import EditIcon from '@mui/icons-material/Edit';
import DeleteIcon from '@mui/icons-material/Delete'
import ChecklistIcon from '@mui/icons-material/Checklist';
import Dialog from '@mui/material/Dialog';
import { useSnackbar } from 'notistack';
import AlertDialog from '../../components/AlertDialog';
import { workflowService } from '../../services/settings/workflowService';
import { originService } from '../../services/settings/originService';
import ListAltIcon from '@mui/icons-material/ListAlt';
import WorkflowAddEditDialog from '../../dialogs/WorkflowAddEditDialog';
import { randomValuesService } from '../../services/utilities/randomValuesService';
import { Proceso } from '../../types/Proceso';
import WorkflowTemplatesDialog from '../../dialogs/WorkflowTemplatesDialog';
import DriveFileMoveIcon from '@mui/icons-material/DriveFileMove';
import WorkflowPhasesDialog from '../../dialogs/WorkflowPhasesDialog';
import { Button, Checkbox, IconButton, Paper, Table, TableBody, TableCell, TableContainer, TableHead, TableRow, ToggleButton, ToggleButtonGroup, Toolbar, Tooltip } from '@mui/material';
import SkeletonTable from '../../components/SkeletonTable';
import WorkflowRequirementsDialog from '../../dialogs/WorkflowRequirementsDialog';
import { useNavigate } from 'react-router-dom';

import InfoIcon from '@mui/icons-material/Info';
import { AssignmentReturned, CompareArrows, DeviceHub, RadioButtonChecked, RadioButtonUnchecked, ThumbDown, ThumbUp, Link, ContentCopy, ReportProblem, RestorePageOutlined } from '@mui/icons-material';
import GroupIcon from '@mui/icons-material/Group';
import ErrorOutlineIcon from '@mui/icons-material/ErrorOutline';
import MessageIcon from '@mui/icons-material/Message';
import { fileService } from '../../services/files/fileService';
import { Expediente } from '../../types/Expediente';
import CircleIcon from '@mui/icons-material/Circle';

import FileCreateDialog from '../../dialogs/FileCreateDialog';
import FileInfoDialog from '../../dialogs/FileInfoDialog';
import FileUnifyDialog from '../../dialogs/FileUnifyDialog';
import exp from 'constants';
import FileCopyDialog from '../../dialogs/FileCopyDialog';
import FileActionLogDialog from '../../dialogs/FileActionLogDialog';
import FileTransferDialog from '../../dialogs/FileTransferDialog';
import FileAssignmentDialog from '../../dialogs/FileAssignmentDialog';

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

export default function Files() {

  const [loading, setLoading] = useState<boolean>(true);
  const [openFileCreateDialog, setOpenFileCreateDialog] = useState<boolean>(false);
  const [openFileInfoDialog, setOpenFileInfoDialog] = useState<boolean>(false);
  const [openUnifyFielsDialog, setOpenUnifyFilesDialog] = useState<boolean>(false);
  const [openFileCopyDialog, setOpenFileCopyDialog] = useState<boolean>(false);
  const [openFileActionLogDialog, setOpenFileActionLogDialog] = useState<boolean>(false);
  const [openFileTraslateLogDialog, setOpenFileTraslateLogDialog] = useState<boolean>(false);
  const [openFileAssignmentDialog, setOpenFileAssignmentDialog] = useState<boolean>(false);
  const [openWorkflowTemplatesDialog, setOpenWorkflowTemplatesDialog] = useState<boolean>(false);
  const [openWorkflowPhasesDialog, setOpenWorkflowPhasesDialog] = useState<boolean>(false);
  const [openWorkflowRequirementsDialog, setOpenWorkflowRequirementsDialog] = useState<boolean>(false);
  const [openWorkflowDeleteDialog, setOpenWorkflowDeleteDialog] = useState<boolean>(false);
  const [selectionMode, setselectionMode] = useState<string>("multiple");
  const [checkboxesChecked, setCheckboxesChecked] = useState<boolean>(false);
  const [selectedFiles, setSelectedFiles] = useState<Expediente[]>([]);
  const [selectedFile, setSelectedFile] = useState<any>();
  const [selectedProcessType, setSelectedProcessType] = useState<number | null>(null);
  const [selectedWorkflow, setSelectedWorkflow] = useState<any>(null);
  const [administrativeUnitslist, setAdministrativeUnitslist] = useState<any>(null);
  const [filesList, setFilesList] = useState<Expediente[]>([]);
  const [filesTotal, setFilesTotal] = useState<number>(0);
  const [originList, setOriginList] = useState<any>([]);
  const [workflowList, setWorkflowList] = useState<Proceso[]>([]);
  const [fileActions, setFileActions] = useState<any>([]);
  const [filePhases, setFilePhases] = useState<any>([]);

  const { enqueueSnackbar } = useSnackbar();
  const navigate = useNavigate();

  const formatFieldId = (fileId: number) => {
    const valueString = fileId.toString();
    const year = valueString.substring(valueString.length - 4);
    const correlative = valueString.substring(0, valueString.length - 4);
    return `${correlative}-${year}`;
  }
  /** Fetch Data Section */

  const fetchFiles = useCallback(async (initialPage: number, itemsPerPage: number, searchString: string) => {
    try {

      setLoading(true);
      const response = await fileService.getAll(initialPage + 1, itemsPerPage, searchString);

      if (response.statusText === 'OK') {        
        setFilesList(response.data.listaExpedientes);
        setFilesTotal(response.data.cantidadTotal);
      }
      else {
        enqueueSnackbar('Ocurrió un error al obtener la lista de procesos.', { variant: 'error' });
      }
    }
    catch (error: any) {
      enqueueSnackbar('Ocurrió un error al obtener la lista de procesos. Detalles: ' + error.message, { variant: 'error' });
    }
    finally {
      setLoading(false);
    }
  }, [enqueueSnackbar]);

  const fetchWorkflows = useCallback(async () => {
    try {
      const response = await workflowService.obtenerProcesosActivos();
      if (response.statusText === 'OK') {
        setWorkflowList(response.data);
      }
      else {
        enqueueSnackbar('Ocurrió un error al obtener la lista de procesos.', { variant: 'error' });
      }
    }
    catch (error: any) {
      enqueueSnackbar('Ocurrió un error al obtener la lista de procesos. Detalles: ' + error.message, { variant: 'error' });
    }
    finally {
      setLoading(false);
    }
  }, [enqueueSnackbar]);

  const fetchOrigins = useCallback(async () => {
    try {
      const response = await originService.getAll(1, 100, '');
      if (response.statusText === 'OK') {
        setOriginList(response.data);
      }
      else {
        enqueueSnackbar('Ocurrió un error al obtener la lista de origenes.', { variant: 'error' });
      }
    }
    catch (error: any) {
      enqueueSnackbar('Ocurrió un error al obtener la lista de origenes. Detalles: ' + error.message, { variant: 'error' });
    }
    finally {
      setLoading(false);
    }
  }, [enqueueSnackbar]);

  const deleteSelectedWorkflow = async (workflowId: number) => {

    setLoading(true);

    try {
      const response = await workflowService.delete(workflowId);

      if (response.statusText === "OK") {
        enqueueSnackbar('Proceso eliminado.', { variant: "success" });
      } else {
        enqueueSnackbar('Ocurrió un Error al eliminar el proceso.', { variant: "error" });
      }
    } catch (error: any) {
      enqueueSnackbar('Ocurrió un Error al eliminar el proceso. Detalles: ' + error.message, { variant: "error" });
    }
    finally {
      setLoading(false);
    }
  }

  const fetchAdministrativeUnits = useCallback(async () => {
    setLoading(true);
    try {
      const response = await administrativeUnitsService.getAll();
      if (response.statusText === 'OK') {
        const administrativeUnits = response.data;

        setAdministrativeUnitslist(administrativeUnits.map((ua: any) => {
          return {
            id: ua.idUnidadAdministrativa, label: ua.nombre
          }
        }));
      }
      else {
        enqueueSnackbar('Ocurrió un Error al obtener las unidades administrativas.', { variant: 'error' });
      }
    }
    catch (error: any) {
      enqueueSnackbar('Ocurrió un Error al obtener las unidades administrativas. Detalles: ' + error.message, { variant: 'error' });
    }
    finally {
      setLoading(false);
    }

    return null;
  }, [enqueueSnackbar]);

  /**************************** Handle Functions Section **********************/

  /* -------------------------  File Create Dialog --------------------------------*/
  const handleOpenFileCreate = async () => {
    setOpenFileCreateDialog(true);
  }

  const handleCloseFileCreateDialog = () => {
    setOpenFileCreateDialog(false);
  }

  const handleCloseFileCreateDialogFromAction = (refreshUsersList: boolean = false) => {
    if (refreshUsersList) {
      fetchFiles(currentPage, rowsPerPage, searchText);
    }
    setOpenFileCreateDialog(false);
  }

  /* -------------------------  File Info Dialog --------------------------------- */
  const handleOpenFileInfo = async (file: any) => {
    setSelectedFile(file);
    setOpenFileInfoDialog(true);
  }

  const handleCloseFileInfoDialog = () => {
    setOpenFileInfoDialog(false);
  }

  const handleCloseFileInfoFromAction = () => {
    setOpenFileInfoDialog(false);
  }
  /* -------------------------  File Unify Dialog ---------------------------------*/
  const handleUnifyFiles = async () => {
    setOpenUnifyFilesDialog(true);
  }

  const handleCloseUnifyFielsDialog = () => {
    setOpenUnifyFilesDialog(false);
  }

  const handleCloseUnifyFielsDialogFromAction = () => {
    setOpenUnifyFilesDialog(false);
  }

  const handleConfirmUnifyFilesDialogFromAction = useCallback(async () => {

    setLoading(true);
    try {

      const currentSelectedFiles = selectedFiles;
      const response = await fileService.unificarExpedientes(currentSelectedFiles);
      if (response.statusText === 'OK') {
        setSelectedFiles([]);
        fetchFiles(currentPage, rowsPerPage, searchText);
        setOpenUnifyFilesDialog(false);
        enqueueSnackbar(' Expedientes Unificados Exitosamente. ', { variant: 'success' });
      } else {
        enqueueSnackbar('Ocurrió un error al Unificar Expedientes. ', { variant: 'error' });
      }
    } catch (error: any) {
      enqueueSnackbar('Ocurrió un error al Unificar Expedientes. Detalles: ' + error.message, { variant: 'error' });
    } finally {
      setLoading(false);
    }
  }, [enqueueSnackbar, selectedFiles]);

  /* -------------------------  File Copy Dialog --------------------------------- */
  const handleOpenFileCopyDialog = async (file: any) => {
    setSelectedFile(file);
    setOpenFileCopyDialog(true);
  }

  const handleCloseFileCopyDialog = () => {
    setOpenFileCopyDialog(false);
  }

  const handleCloseFileCopyDialogFromAction = () => {
    setOpenFileCopyDialog(false);
  }

  const handleConfirmFileCopyDialogFromAction = useCallback(async (numeroCopias: number) => {

    setLoading(true);
    try {

      const currentSelectedFile = selectedFile;

      if (currentSelectedFile) {

        const copyFile = {
          idExpediente: currentSelectedFile.idExpediente,
          numeroCopias: numeroCopias
        }

        const response = await fileService.copiarExpediente(copyFile);
        if (response.statusText === 'OK') {
          fetchFiles(currentPage, rowsPerPage, searchText);
          setOpenUnifyFilesDialog(false);
          enqueueSnackbar(' Expedientes Copiados Exitosamente. ', { variant: 'success' });
        } else {
          enqueueSnackbar('Ocurrió un error al Copiar Expedientes. ', { variant: 'error' });
        }
      }
    } catch (error: any) {
      enqueueSnackbar('Ocurrió un error al Copiar Expedientes. Detalles: ' + error.message, { variant: 'error' });
    } finally {
      setLoading(false);
    }
  }, [enqueueSnackbar, selectedFile]);

  /* -------------------------  File Action Dialog --------------------------------- */
  const handleOpenFileActionLogDialog = async (file: any) => {
    setSelectedFile(file);
    fetchFileActions(file);
    setOpenFileActionLogDialog(true);
  }

  const fetchFileActions = async (file: any) => {

    const objAntonatioFile = {
      idEntidad: file.idEntidad,
      idProceso: file.idProceso,
      idExpediente: file.idExpediente,
      idFaseActual: file.idFaseActual,
      fechaTraslado: file.fechaTraslado,
      fechaAsignacion: file.fechaAsignacion
    }

    try {
      const response = await fileService.obtenerAnotaciones(objAntonatioFile);
      if (response.statusText === 'OK') {
        const fileActions = response.data;
        setFileActions(fileActions);
      }
      else {
        enqueueSnackbar('Ocurrió un Error al obtener las acciones del expediente.', { variant: 'error' });
      }
    }
    catch (error: any) {
      enqueueSnackbar('Ocurrió un Error al obtener las acciones del expediente. Detalles: ' + error.message, { variant: 'error' });
    }
  }

  const handleCloseFileActionLogDialog = () => {
    setOpenFileActionLogDialog(false);
  }

  const handleCloseFileActionLogDialogFromAction = () => {
    setOpenFileActionLogDialog(false);
  }

  const handleConfirmFileActionLogDialogFromAction = useCallback(async (accion: any) => {

    setLoading(true);
    try {

      const currentSelectedFile = selectedFile;

      if (currentSelectedFile) {

        const anotationFileObject = {
          idEntidad: currentSelectedFile.idEntidad,
          idProceso: currentSelectedFile.idProceso,
          idExpediente: currentSelectedFile.idExpediente,
          idFase: currentSelectedFile.idFaseActual,
          idPrivacidad: accion.idPrivacidad,
          fechaTraslado: currentSelectedFile.fechaTraslado,
          fechaAsignacion: currentSelectedFile.fechaAsignacion,
          idAnotacion: 0, // Default value for new annotation
          anotacion: accion.observacion
        }

        const response = await fileService.agregarAnotacion(anotationFileObject);
        if (response.statusText === 'OK') {
          fetchFiles(currentPage, rowsPerPage, searchText);
          setOpenUnifyFilesDialog(false);
          fetchFileActions(currentSelectedFile);
          enqueueSnackbar(' Observación registrada exitosamente ', { variant: 'success' });
        } else {
          enqueueSnackbar('Ocurrió un error al registrar la observación. ', { variant: 'error' });
        }
      }
    } catch (error: any) {
      enqueueSnackbar('Ocurrió un error al registrar la observación. Detalles: ' + error.message, { variant: 'error' });
    } finally {
      setLoading(false);
    }
  }, [enqueueSnackbar, selectedFile]);

  const handleDeleteActionFile = async (anotation: any, actionResult: boolean) => {

    const currentSelectedFile = selectedFile;
    try {
      if (actionResult) {
        await fileService.eliminarAnortacion(anotation);
        fetchFileActions(currentSelectedFile);
        enqueueSnackbar("Observación eliminada.", { variant: "success" });
      }
    }
    catch (error) {
      enqueueSnackbar("Ocurrió un error al eliminar la observación.", { variant: "error" });
    }
  }

  /* -------------------------  File Traslate Dialog --------------------------------- */

  const handleOpenTransferFileDialog = async (file: any) => {
    setSelectedFile(file);
    fetchFilePhases(file.idExpediente);
    setOpenFileTraslateLogDialog(true);
  }

  const handleCloseFileTraslateLogDialog = () => {
    setOpenFileTraslateLogDialog(false);
  }

    const fetchFilePhases = useCallback(async (idExpediente: number) => {
    setLoading(true);
    try {
      const response = await fileService.obtenerFasesSiguientes(idExpediente);
      if (response.statusText === 'OK') {        
        setFilePhases(response.data);
      }
      else {
        enqueueSnackbar('Ocurrió un Error al obtener las fases del expediente.', { variant: 'error' });
      }
    }
    catch (error: any) {
      enqueueSnackbar('Ocurrió un Error al obtener las fases del expediente. Detalles: ' + error.message, { variant: 'error' });
    }
    finally {
      setLoading(false);
    }

    return null;
  }, [enqueueSnackbar]);

  const handleConfirmTransferFileDialogFromAction = useCallback(async (data: any) => {

    setLoading(true);
    try {

      const currentSelectedFile = selectedFile;

      console.log("data: ", data);
      if (currentSelectedFile) {

        const fileObject = {
          idEntidad: currentSelectedFile.idEntidad,
          idProceso: currentSelectedFile.idProceso,
          idExpediente: currentSelectedFile.idExpediente,
          idFase: currentSelectedFile.idFaseActual,
          idFaseDestino: data.faseDestino.idFase,
          faseDestino: data.faseDestino.fase,
          idUsuarioAsignado: data.idUsuarioAsignado,
          fechaLimiteAtencion: data.fechaLimiteAtencion,
          observacion: data.observaciones
        }

        const response = await fileService.trasladarExpediente(fileObject);
        if (response.statusText === 'OK') {
          fetchFiles(currentPage, rowsPerPage, searchText);
          setOpenFileTraslateLogDialog(false);          
          fetchFileActions(currentSelectedFile);
          enqueueSnackbar(' Traslado realizado exitosamente ', { variant: 'success' });
        } else {
          enqueueSnackbar('Ocurrió un error al trasladar el expediente. ', { variant: 'error' });
        }
      }
    } catch (error: any) {
      enqueueSnackbar('Ocurrió un error al trasladar el expediente. Detalles: ' + error.message, { variant: 'error' });
    } finally {
      setLoading(false);
    }
  }, [enqueueSnackbar, selectedFile]);

  /* -------------------------  File Assignment Dialog --------------------------------- */

  const handleOpenAssignmentFileDialog = async (file: any) => {
    setSelectedFile(file);
    fetchFilePhases(file.idExpediente);
    setOpenFileAssignmentDialog(true);
  }

  const handleCloseFileAssignmentDialog = () => {
    setOpenFileAssignmentDialog(false);
  }

  const handleConfirmAssignmentFileDialogFromAction = useCallback(async (data: any) => {

    setLoading(true);
    try {

      const currentSelectedFile = selectedFile;

      console.log("data: ", data);
      if (currentSelectedFile) {

        const fileObject = {
          idEntidad: currentSelectedFile.idEntidad,
          idProceso: currentSelectedFile.idProceso,
          idExpediente: currentSelectedFile.idExpediente,
          idFase: currentSelectedFile.idFaseActual,
          idFaseDestino: data.faseDestino.idFase,
          faseDestino: data.faseDestino.fase,
          idUsuarioAsignado: data.idUsuarioAsignado,
          fechaLimiteAtencion: data.fechaLimiteAtencion,
          observacion: data.observaciones
        }

        const response = await fileService.trasladarExpediente(fileObject);
        if (response.statusText === 'OK') {
          fetchFiles(currentPage, rowsPerPage, searchText);
          setOpenFileTraslateLogDialog(false);          
          fetchFileActions(currentSelectedFile);
          enqueueSnackbar(' Traslado realizado exitosamente ', { variant: 'success' });
        } else {
          enqueueSnackbar('Ocurrió un error al trasladar el expediente. ', { variant: 'error' });
        }
      }
    } catch (error: any) {
      enqueueSnackbar('Ocurrió un error al trasladar el expediente. Detalles: ' + error.message, { variant: 'error' });
    } finally {
      setLoading(false);
    }
  }, [enqueueSnackbar, selectedFile]);

  /* ------------------------------------------------------------------------------ */

  const handleSelectionMode = async (mode: string) => {
    setselectionMode(mode);
    setCheckboxesChecked(false);
    setSelectedFiles([]);
    setSelectedProcessType(null);

    if (mode === "multiple") {
      enqueueSnackbar("Ahora podrás seleccionar expedientes de diferentes tipos de proceso para vincularlos", { variant: "info" });
    } else {
      enqueueSnackbar("Ahora únicamente podrás seleccionar expedientes de un mismo tipo para operaciones masivas", { variant: "info" });
    }
  }

  const handleSelectedWorkflowEdit = async (workflow: any) => {
    //setOpenWorkflowAddEditDialog(true);
  }

  // User Disable dialog
  const handleOpenWorkflowTemplatesDialog = async (workflow: any) => {
    setSelectedWorkflow(workflow);
    setOpenWorkflowTemplatesDialog(true);
  }

  const handleCloseWorkflowTemplatesDialog = () => {
    setOpenWorkflowTemplatesDialog(false);
  }

  const handleCloseWorkflowTemplatesDialogFromAction = (refreshWorkflowsList: boolean = false) => {
    if (refreshWorkflowsList) {
      //fetchWorkflows(currentPage, rowsPerPage, searchText);
    }
    setOpenWorkflowTemplatesDialog(false);
  }

  // Worflow Delete Alert dialog
  const handleOpenWorkflowDeleteDialog = async (workflow: any) => {
    setSelectedWorkflow(workflow);
    setOpenWorkflowDeleteDialog(true);
  }

  const handleCloseWorkflowDeleteDialog = () => {
    setOpenWorkflowDeleteDialog(false);
  }

  const handleCloseWorkflowDeleteDialogFromAction = async (actionResult: boolean = false) => {
    if (actionResult) {
      await deleteSelectedWorkflow(selectedWorkflow.idProceso);
      //await fetchWorkflows(currentPage, rowsPerPage, searchText);
    }
    setOpenWorkflowDeleteDialog(false);
  }

  // Phases and transitions dialog
  const handleWorkflowPhasesDialog = async (workflow: any) => {
    setSelectedWorkflow(workflow);
    setOpenWorkflowPhasesDialog(true);
  }

  const handleCloseWorkflowPhasesDialog = () => {
    setOpenWorkflowPhasesDialog(false);
  }

  const handleCloseWorkflowPhasesDialogFromAction = (refreshWorkflowsList: boolean = false) => {
    if (refreshWorkflowsList) {
      //fetchWorkflows(currentPage, rowsPerPage, searchText);
    }
    setOpenWorkflowPhasesDialog(false);
  }

  // Creation requirements
  const handleWorkflowRequirementsDialog = async (workflow: any) => {
    setSelectedWorkflow(workflow);
    setOpenWorkflowRequirementsDialog(true);
  }

  const handleCloseWorkflowRequirementsDialog = () => {
    setOpenWorkflowRequirementsDialog(false);
  }

  const handleCloseWorkflowRequirementsDialogFromAction = (refreshWorkflowsList: boolean = false) => {
    if (refreshWorkflowsList) {
      // fetchWorkflows(currentPage, rowsPerPage, searchText);
    }
    setOpenWorkflowRequirementsDialog(false);
  }

  const handleSelectAll = (event: React.ChangeEvent<HTMLInputElement>) => {
    if (event.target.checked) {
      const allFileIds = filesList.map(file => file.idExpediente);
      //-----------------------------------------setSelectedFiles(allFileIds);
    } else {
      setSelectedFiles([]);
    }
    setCheckboxesChecked(event.target.checked);
  };

  const handleSelectOne = (file: Expediente) => {
    setSelectedFiles(prev => {
      // Si el modo de selección es "tipos", solo permitimos seleccionar expedientes del mismo tipo
      if (selectionMode === "tipos") {
        // Verificamos si ya hay un expediente seleccionado
        if (prev.length > 0) {
          // Obtenemos el tipo del primer expediente seleccionado
          const firstSelectedType = prev[0].idProceso;

          // Si el expediente que se intenta seleccionar es de un tipo diferente, no lo agregamos
          if (file.idProceso !== firstSelectedType) {
            enqueueSnackbar('Solo puedes seleccionar expedientes del mismo tipo.', { variant: 'warning' });
            return prev; // Retornamos el estado anterior sin cambios (Expediente[])
          }
        }
      }

      // Verificamos si el archivo ya está en el array usando el idExpediente
      const exists = prev.some(existingFile => existingFile.idExpediente === file.idExpediente);

      if (exists) {
        // Si existe, lo eliminamos
        const newSelectedFiles = prev.filter(existingFile => existingFile.idExpediente !== file.idExpediente);

        // Si estamos en modo "tipos" y no quedan expedientes seleccionados, reseteamos el tipo seleccionado
        if (selectionMode === "tipos" && newSelectedFiles.length === 0) {
          setSelectedProcessType(null);
        }

        return newSelectedFiles; // Expediente[]
      } else {
        // Si no existe, lo agregamos
        const newSelectedFiles = [...prev, file];

        // Si estamos en modo "tipos", actualizamos el tipo de proceso seleccionado
        if (selectionMode === "tipos") {
          setSelectedProcessType(file.idProceso);
        }

        return newSelectedFiles; // Expediente[]
      }
    });
  };


  useEffect(() => {
    fetchFiles(currentPage, rowsPerPage, searchText);
    fetchWorkflows();
    fetchOrigins();
    fetchAdministrativeUnits();

  }, [fetchWorkflows, fetchAdministrativeUnits, fetchFiles]);

  /** Return Section */
  return (
    <>
      <Page title="Listado de Expedientes">
        <Toolbar style={{ paddingLeft: "0px", justifyContent: "flex-end" }}>
          {/* <Button disableElevation onClick={() => { handleOpenWorkflowAddEditDialog(); }}>
            <AddIcon fontSize="small" /> Nuevo Expediente	
          </Button> */}
          {selectionMode === 'tipos' && (
            <IconButton disabled={false} onClick={() => handleSelectionMode("multiple")}>
              <Tooltip title={'Habilitar Selección Múltiple para Unificación de Expedientes'} arrow placement="top">
                <RadioButtonChecked />
              </Tooltip>
            </IconButton>
          )}
          {selectionMode === 'multiple' && (
            <IconButton disabled={false} onClick={() => handleSelectionMode("tipos")}>
              <Tooltip title={'Habilitar Selección por Tipo de Proceso para Acciones Masivas'} arrow placement="top">
                <RadioButtonUnchecked />
              </Tooltip>
            </IconButton>
          )}
          {selectionMode === 'multiple' && selectedFiles.length > 1 && (
            <IconButton onClick={() => handleUnifyFiles()}>
              <Tooltip title={'Unificar Expedientes'} arrow placement="top">
                <DeviceHub />
              </Tooltip>
            </IconButton>
          )}
          <IconButton disabled={true} onClick={() => handleSelectedWorkflowEdit(null)}>
            <Tooltip title={'Confirmar Asignación'} arrow placement="top">
              <ThumbUp />
            </Tooltip>
          </IconButton>
          <IconButton disabled={true} onClick={() => handleSelectedWorkflowEdit(null)}>
            <Tooltip title={'Rechazar Asignación'} arrow placement="top">
              <ThumbDown />
            </Tooltip>
          </IconButton>
          {selectionMode === 'tipos' && selectedFiles.length > 0 && (
            <IconButton onClick={() => handleSelectedWorkflowEdit(null)}>
              <Tooltip title={'Traslado Masivo de Expedientes'} arrow placement="top">
                <CompareArrows />
              </Tooltip>
            </IconButton>
          )}
          {selectionMode === 'tipos' && selectedFiles.length > 0 && (
            <IconButton onClick={() => handleSelectedWorkflowEdit(null)}>
              <Tooltip title={'Asignación Interna Masiva de Expedientes'} arrow placement="top">
                <GroupIcon />
              </Tooltip>
            </IconButton>
          )}
          <IconButton onClick={() => handleOpenFileCreate()}>
            <Tooltip title={'Creación de Expediente'} arrow placement="top">
              <AddIcon />
            </Tooltip>
          </IconButton>
        </Toolbar>
        <TableContainer component={Paper} sx={{ maxHeight: 610 }}>
          {/* TODO: Search  */}
          <Table stickyHeader aria-label="workflow list" size="small">
            <TableHead>
              <TableRow>
                <TableCell style={{ minWidth: 20 }}>
                  {selectionMode === 'multiple' && (
                    <Checkbox
                      checked={checkboxesChecked}
                      onChange={handleSelectAll}
                      indeterminate={selectedFiles.length > 0 && selectedFiles.length < filesList.length}
                    />
                  )}
                </TableCell>
                <TableCell style={{ minWidth: 5 }}>
                  <b>No. Expediente</b>
                </TableCell>
                <TableCell style={{ minWidth: 5 }}>
                  <b>Descripción</b>
                </TableCell>
                <TableCell style={{ minWidth: 100 }}>
                  <b>Origen</b>
                </TableCell>
                <TableCell style={{ minWidth: 50 }}>
                  <b>Emisor</b>
                </TableCell>
                <TableCell style={{ minWidth: 100 }}>
                  <b>Fecha de Asignación</b>
                </TableCell>
                <TableCell style={{ minWidth: 100 }}>
                  <b>Proceso</b>
                </TableCell>
                <TableCell style={{ minWidth: 100 }}>
                  <b>Fase Actual del Proceso</b>
                </TableCell>
                <TableCell style={{ minWidth: 20 }}>
                  <b>Semáforo</b>
                </TableCell>
                <TableCell style={{ minWidth: 300 }}>
                  <b>Acciones</b>
                </TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {
                loading ?
                  <SkeletonTable columnsNumber={9} rowsNumber={3} />
                  :
                  filesList?.map((file, index) => (
                    <TableRow hover key={index}>
                      <TableCell style={{ minWidth: 20 }}>
                        <Checkbox {...label}
                          checked={selectedFiles.includes(file)}
                          onChange={() => handleSelectOne(file)}
                          disabled={selectionMode === "tipos" &&
                            selectedProcessType !== null &&
                            file.idProceso !== selectedProcessType}
                          sx={{
                            color: file.colorProceso,
                            '&.Mui-checked': {
                              color: file.colorProceso,
                            }
                          }} />
                      </TableCell>
                      <TableCell style={{ minWidth: 5 }}>
                        <div style={{ display: "inline" }}>
                          <div
                            style={{
                              display: "inline-block",
                              float: "left",
                              width: "2mm",
                              height: "5mm",
                              backgroundColor: file.colorProceso,
                            }}
                          ></div>
                          <div style={{ display: "inlineBlock", marginLeft: "21px" }}>
                            <button
                              type="button"
                              style={{
                                background: 'none',
                                border: 'none',
                                padding: 0,
                                margin: 0,
                                color: 'inherit',
                                textDecoration: 'underline',
                                cursor: 'pointer',
                                font: 'inherit'
                              }}
                              onClick={() => {
                                navigate(`/files/details/${file.idExpediente}`);
                              }}
                            >
                              {formatFieldId(file.idExpediente)}
                            </button>
                          </div>
                        </div>
                      </TableCell>
                      <TableCell style={{ minWidth: 100 }}>
                        {file.descripcion}
                      </TableCell>
                      <TableCell style={{ minWidth: 100 }}>
                        {file.nombreOrigen}
                      </TableCell>
                      <TableCell style={{ minWidth: 100 }}>
                        {file.emisor}
                      </TableCell>
                      <TableCell style={{ minWidth: 100 }}>
                        {file.fechaAsignacion ? new Date(file.fechaAsignacion).toLocaleDateString() : 'N/A'}
                      </TableCell>
                      <TableCell style={{ minWidth: 100 }}>
                        {file.nombreProceso}
                      </TableCell>
                      <TableCell style={{ minWidth: 100 }}>
                        {file.faseActualProceso}
                      </TableCell>
                      <TableCell style={{ minWidth: 20, textAlign: 'center', verticalAlign: 'middle' }}>
                        <span>
                          {file.porcentajeTiempoTranscurrido === -1 && (
                            <CircleIcon sx={{ color: theme => theme.palette.success.main }} />
                          )}

                          {file.porcentajeTiempoTranscurrido >= 0 &&
                            file.porcentajeTiempoTranscurrido < 75 && (
                              <CircleIcon sx={{ color: theme => theme.palette.success.main }} />
                            )}

                          {file.porcentajeTiempoTranscurrido >= 75 &&
                            file.porcentajeTiempoTranscurrido <= 90 && (
                              <CircleIcon sx={{ color: theme => theme.palette.warning.main }} />
                            )}

                          {(file.porcentajeTiempoTranscurrido >= 90 ||
                            file.porcentajeTiempoTranscurrido < -1) && (
                              <CircleIcon sx={{ color: theme => theme.palette.error.main }} />
                            )}
                        </span>
                      </TableCell>
                      <TableCell style={{ minWidth: 300 }}>
                        <IconButton onClick={() => handleOpenFileInfo(file)}>
                          <Tooltip title={'Información de Asignación'} arrow placement="top">
                            <InfoIcon />
                          </Tooltip>
                        </IconButton>
                        <IconButton disabled={file.idTipoOperacion === 2} onClick={() => handleOpenTransferFileDialog(file)}>
                          <Tooltip title={'Traslado Expedientes'} arrow placement="top">
                            <CompareArrows />
                          </Tooltip>
                        </IconButton>
                        <IconButton disabled={file.idTipoOperacion === 2} onClick={() => handleOpenAssignmentFileDialog(file)}>
                          <Tooltip title={'Asignación Interna'} arrow placement="top">
                            <GroupIcon />
                          </Tooltip>
                        </IconButton>
                        <IconButton disabled={file.idTipoOperacion === 2}onClick={() => handleOpenFileActionLogDialog(file)}>
                          <Tooltip title={'Bitácora de Acciones'} arrow placement="top">
                            <MessageIcon />
                          </Tooltip>
                        </IconButton>
                        <IconButton disabled={file.idTipoOperacion === 2} onClick={() => handleOpenWorkflowDeleteDialog(null)}>
                          <Tooltip title={'Asignarme el expediente'} arrow placement="top">
                            <AssignmentReturned />
                          </Tooltip>
                        </IconButton>
                        <IconButton disabled={file.idTipoOperacion !== 2} onClick={() => handleOpenWorkflowDeleteDialog(null)}>
                          <Tooltip title={'Confirmar Recepción'} arrow placement="top">
                            <ThumbUp />
                          </Tooltip>
                        </IconButton>
                        <IconButton disabled={file.idTipoOperacion !== 2} onClick={() => handleOpenWorkflowDeleteDialog(null)}>
                          <Tooltip title={'Rechazar Recepción'} arrow placement="top">
                            <ThumbDown />
                          </Tooltip>
                        </IconButton>
                        <IconButton disabled={file.idTipoOperacion === 2} onClick={() => handleOpenWorkflowDeleteDialog(null)}>
                          <Tooltip title={'Vinculaciones'} arrow placement="top">
                            <Link />
                          </Tooltip>
                        </IconButton>
                        <IconButton disabled={file.idTipoOperacion === 2} onClick={() => handleOpenFileCopyDialog(file)}>
                          <Tooltip title={'Crear Copias del Expediente'} arrow placement="top">
                            <ContentCopy />
                          </Tooltip>
                        </IconButton>
                        {(file.idTipoOperacion === 1 && file.ultimaIdTipoOperacion === 2) && (
                        <IconButton>
                          <Tooltip title={'Expediente Enviado - Pendiente de Recepción'} arrow placement="top">
                            <ErrorOutlineIcon color="warning" />
                          </Tooltip>
                        </IconButton>
                        )}
                      </TableCell>
                    </TableRow>
                  ))}
            </TableBody>
          </Table>
        </TableContainer>
      </Page>

      {/* ------------------ Crear Expediente ----------------------- */}
      <Dialog
        open={openFileCreateDialog}
        onClose={handleCloseFileCreateDialog}
        maxWidth={"lg"}
      >
        <FileCreateDialog
          procesos={workflowList}
          origenes={originList}
          onClose={handleCloseFileCreateDialogFromAction}
        />
      </Dialog>
      {/* //------------------- Información del Expediente ----------------------- */}
      <Dialog
        open={openFileInfoDialog}
        onClose={handleCloseFileInfoDialog}
        maxWidth={"lg"}
      >
        <FileInfoDialog
          file={selectedFile}
          onClose={handleCloseFileInfoFromAction}
        />
      </Dialog>
      {/* //------------------- Unificar Expedientes ----------------------- */}
      <Dialog
        open={openUnifyFielsDialog}
        onClose={handleCloseUnifyFielsDialog}
        maxWidth={"lg"}
      >
        <FileUnifyDialog
          file={selectedFile}
          open={openUnifyFielsDialog}
          onConfirm={handleConfirmUnifyFilesDialogFromAction}
          onClose={handleCloseUnifyFielsDialogFromAction}
        />
      </Dialog>

      {/* ------------------- Copiar Expedientes ----------------------- */}
      <Dialog
        open={openFileCopyDialog}
        onClose={handleCloseFileCopyDialog}
        maxWidth={"lg"}
      >
        <FileCopyDialog
          file={selectedFile}
          open={openFileCopyDialog}
          onClose={handleCloseFileCopyDialogFromAction}
          onSubmitCopias={handleConfirmFileCopyDialogFromAction}
        />
      </Dialog>
      {/* ------------------- Bitacora de Acciones Expedientes ----------------------- */}
      <Dialog
        open={openFileActionLogDialog}
        onClose={handleCloseFileActionLogDialog}
        maxWidth={"lg"}
      >
        <FileActionLogDialog
          open={openFileActionLogDialog}
          file={selectedFile}
          onClose={handleCloseFileActionLogDialogFromAction}
          expedienteLabel={selectedFile?.numeroExpediente}
          onAddObservation={handleConfirmFileActionLogDialogFromAction}
          onDeleteObservation={handleDeleteActionFile}
          anotation={fileActions}
        />
      </Dialog>
      {/* ------------------- Traslado de Expedientes ----------------------- */}
      <Dialog
        open={openFileTraslateLogDialog}
        onClose={handleCloseFileTraslateLogDialog}
        maxWidth={"lg"}
      >
        <FileTransferDialog
          open={openFileTraslateLogDialog}
          file={selectedFile}
          onClose={handleCloseFileTraslateLogDialog}
          onSubmit={handleConfirmTransferFileDialogFromAction}
          fases={filePhases}          
        />
      </Dialog>

      {/* ------------------- Asignación de Expedientes ----------------------- */}
      <Dialog
        open={openFileAssignmentDialog}
        onClose={handleCloseFileAssignmentDialog}
        maxWidth={"lg"}
      >
        <FileAssignmentDialog
          open={openFileAssignmentDialog}
          file={selectedFile}
          onClose={handleCloseFileAssignmentDialog}
          onSubmit={handleConfirmAssignmentFileDialogFromAction}            
        />
      </Dialog>

    </>
  );
}


