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
import ListAltIcon from '@mui/icons-material/ListAlt';
import WorkflowAddEditDialog from '../../dialogs/WorkflowAddEditDialog';
import { randomValuesService } from '../../services/utilities/randomValuesService';
import { Proceso } from '../../types/Proceso';
import WorkflowTemplatesDialog from '../../dialogs/WorkflowTemplatesDialog';
import DriveFileMoveIcon from '@mui/icons-material/DriveFileMove';
import WorkflowPhasesDialog from '../../dialogs/WorkflowPhasesDialog';
import { Button, Checkbox, IconButton, Paper, Table, TableBody, TableCell, TableContainer, TableHead, TableRow, Toolbar, Tooltip } from '@mui/material';
import SkeletonTable from '../../components/SkeletonTable';
import WorkflowRequirementsDialog from '../../dialogs/WorkflowRequirementsDialog';

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

export default function Workflows() {

  const [loading, setLoading] = useState<boolean>(true);
  const [openWorkflowAddEditDialog, setOpenWorkflowAddEditDialog] = useState<boolean>(false);
  const [openWorkflowTemplatesDialog, setOpenWorkflowTemplatesDialog] = useState<boolean>(false);
  const [openWorkflowPhasesDialog, setOpenWorkflowPhasesDialog] = useState<any>(null);
  const [openWorkflowRequirementsDialog, setOpenWorkflowRequirementsDialog] = useState<any>(null);
  const [openWorkflowDeleteDialog, setOpenWorkflowDeleteDialog] = useState<boolean>(false);
  const [selectedWorkflow, setSelectedWorkflow] = useState<any>(null);
  const [administrativeUnitslist, setAdministrativeUnitslist] = useState<any>(null);
  const [workflowList, setWorkflowList] = useState<Proceso[]>([]); 

  const { enqueueSnackbar } = useSnackbar();

  /** Fetch Data Section */

  const fetchWorkflows = useCallback(async (initialPage: number, itemsPerPage: number, searchString: string) => {
    try {
      setLoading(true);
      const response = await workflowService.getAll(initialPage + 1, itemsPerPage, searchString);

      if(response.statusText === 'OK') {
        setWorkflowList(response.data);        
      }
      else {
        enqueueSnackbar('Ocurrió un error al obtener la lista de procesos.', { variant: 'error' });
      }        
    }
    catch(error: any){
      enqueueSnackbar('Ocurrió un error al obtener la lista de procesos. Detalles: ' + error.message, { variant: 'error' });      
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
  
  const fetchAdministrativeUnits = useCallback(async () =>{ 

    setLoading(true);
    try {
      const response = await administrativeUnitsService.getAll();
      if(response.statusText === 'OK') {
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
    catch(error: any){
      enqueueSnackbar('Ocurrió un Error al obtener las unidades administrativas. Detalles: ' + error.message, { variant: 'error' });
    }
    finally {
      setLoading(false);
    }
    
    return null;    
  }, [enqueueSnackbar]); 

  /** Handle Functions Section */

  // Workflow Add/Edit dialog
  const handleOpenWorkflowAddEditDialog = () => {
    setSelectedWorkflow(emptyWorkflowObject);
    setOpenWorkflowAddEditDialog(true);
  }

  const handleCloseUserAddEditDialog = () => {
    setOpenWorkflowAddEditDialog(false);
  }

  const handleCloseUserAddEditDialogFromAction = (refreshUsersList: boolean = false) => {
    if(refreshUsersList) {
      fetchWorkflows(currentPage, rowsPerPage, searchText);
    }
    setOpenWorkflowAddEditDialog(false);
  }

  const handleSelectedWorkflowEdit = async (workflow: any) => {       
    setSelectedWorkflow(workflow);
    setOpenWorkflowAddEditDialog(true);
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
    if(refreshWorkflowsList) {
      fetchWorkflows(currentPage, rowsPerPage, searchText);
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
    if(actionResult) {
      await deleteSelectedWorkflow(selectedWorkflow.idProceso);
      await fetchWorkflows(currentPage, rowsPerPage, searchText);
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
    if(refreshWorkflowsList) {
      fetchWorkflows(currentPage, rowsPerPage, searchText);
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
    if(refreshWorkflowsList) {
      fetchWorkflows(currentPage, rowsPerPage, searchText);
    }
    setOpenWorkflowRequirementsDialog(false);
  }

  useEffect(() => {

    fetchWorkflows(currentPage, rowsPerPage, searchText);
    fetchAdministrativeUnits(); 

  }, [fetchWorkflows, fetchAdministrativeUnits]);

  /** Return Section */
  return (
    <>
      <Page title="Procesos">
        <Toolbar style={{ paddingLeft: "0px", justifyContent: "flex-end" }}>          
          <Button disableElevation onClick={() => { handleOpenWorkflowAddEditDialog(); }}>
            <AddIcon fontSize="small" /> Nuevo Proceso	
          </Button>
        </Toolbar>
        <TableContainer component={Paper} sx={{ maxHeight: 610 }}>
          <Table stickyHeader aria-label="workflow list" size="small">
            <TableHead>
              <TableRow>
                <TableCell style={{ minWidth: 5 }}>
                    <b>Nombre</b>
                </TableCell>
                <TableCell style={{ minWidth: 5 }}>
                    <b>Descripción</b>
                </TableCell>
                <TableCell style={{ minWidth: 100 }}>
                    <b>Unidad Administrativa</b>
                </TableCell>
                <TableCell style={{ minWidth: 50 }}>
                    <b>Tipo</b>
                </TableCell>
                <TableCell style={{ minWidth: 100 }}>
                    <b>Estado</b>
                </TableCell>
                <TableCell style={{ minWidth: 100 }}>
                    <b>Acciones</b>
                </TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {
                loading ? 
                <SkeletonTable columnsNumber={6} rowsNumber={3} />
                :               
                workflowList?.map((workflow, index) => (
                <TableRow hover key={index}>
                  <TableCell style={{ minWidth: 5 }}>
                    <div style={{ display: "inline" }}>
                      <div
                          style={{
                            display: "inline-block",
                            float: "left",
                            width: "2mm",
                            height: "5mm",
                            backgroundColor: workflow.color,
                          }}
                      ></div>
                      <div style={{ display: "inlineBlock", marginLeft: "21px" }}>
                        {workflow.nombre}
                      </div>
                    </div>
                  </TableCell>
                  <TableCell style={{ minWidth: 100 }}>
                    {workflow.descripcion}
                  </TableCell>
                  <TableCell style={{ minWidth: 100 }}>
                    {workflow.unidadAdministrativa}
                  </TableCell>
                  <TableCell style={{ minWidth: 100 }}>
                    {workflow.tipoProceso}
                  </TableCell>
                  <TableCell style={{ minWidth: 100 }}>
                    <Checkbox checked={workflow.estado} disabled={true} />
                  </TableCell>
                  <TableCell style={{ minWidth: 100 }}>
                    <IconButton onClick={() => handleSelectedWorkflowEdit(workflow)}>
                      <Tooltip title={'Editar Proceso'} arrow placement="top"> 
                        <EditIcon />
                      </Tooltip>                    
                    </IconButton>
                    <IconButton onClick={() => handleOpenWorkflowTemplatesDialog(workflow)}>
                      <Tooltip title={'Plantillas'} arrow placement="top"> 
                        <ListAltIcon />
                      </Tooltip>                    
                    </IconButton>
                    <IconButton onClick={() => handleWorkflowPhasesDialog(workflow)}>
                      <Tooltip title={'Fases y Transiciones'} arrow placement="top"> 
                        <DriveFileMoveIcon />
                      </Tooltip>                    
                    </IconButton>
                    <IconButton onClick={() => handleWorkflowRequirementsDialog(workflow)}>
                      <Tooltip title={'Requisitos de Creación'} arrow placement="top"> 
                        <ChecklistIcon />
                      </Tooltip>                    
                    </IconButton>
                    <IconButton onClick={() => handleOpenWorkflowDeleteDialog(workflow)}>
                      <Tooltip title={'Eliminar Proceso'} arrow placement="top"> 
                        <DeleteIcon />
                      </Tooltip>
                    </IconButton>
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </TableContainer>          
      </Page>

      <Dialog
        open={openWorkflowAddEditDialog}
        onClose={handleCloseUserAddEditDialog}
        maxWidth={"lg"}        
      >
        <WorkflowAddEditDialog 
          mode = {selectedWorkflow && selectedWorkflow.idProceso > -1 ? 'edit' : 'add'}
          selectedWorkflow = {selectedWorkflow}
          administrativeUnitsList = {administrativeUnitslist}
          onClose = {handleCloseUserAddEditDialogFromAction}
        />        
      </Dialog>

      <Dialog
        open={openWorkflowTemplatesDialog}
        onClose={handleCloseWorkflowTemplatesDialog}
        maxWidth={"lg"}
      >
        <WorkflowTemplatesDialog 
          selectedWorkflow = {selectedWorkflow}          
          onClose = {handleCloseWorkflowTemplatesDialogFromAction}
        />        
      </Dialog>

      <Dialog
        open={openWorkflowPhasesDialog}
        onClose={handleCloseWorkflowPhasesDialog}
        maxWidth={"lg"}
      >
        <WorkflowPhasesDialog 
          selectedWorkflow = {selectedWorkflow}          
          onClose = {handleCloseWorkflowPhasesDialogFromAction}
        />        
      </Dialog>

      <Dialog
        open={openWorkflowRequirementsDialog}
        onClose={handleCloseWorkflowRequirementsDialog}
        maxWidth={"lg"}
      >
        <WorkflowRequirementsDialog 
          selectedWorkflow = {selectedWorkflow}          
          onClose = {handleCloseWorkflowRequirementsDialogFromAction}
        />        
      </Dialog>

      <Dialog
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
      </Dialog>
          
    </>    
  );
}
