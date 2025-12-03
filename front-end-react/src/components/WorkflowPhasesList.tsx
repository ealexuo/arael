import * as React from 'react';
import Box from '@mui/material/Box';
import Collapse from '@mui/material/Collapse';
import IconButton from '@mui/material/IconButton';
import Table from '@mui/material/Table';
import TableBody from '@mui/material/TableBody';
import TableCell from '@mui/material/TableCell';
import TableContainer from '@mui/material/TableContainer';
import TableHead from '@mui/material/TableHead';
import TableRow from '@mui/material/TableRow';
import Paper from '@mui/material/Paper';
import KeyboardArrowDownIcon from '@mui/icons-material/KeyboardArrowDown';
import KeyboardArrowUpIcon from '@mui/icons-material/KeyboardArrowUp';
import { Alert, Button, Checkbox, Dialog, Toolbar, Tooltip } from '@mui/material';
import DeleteIcon from '@mui/icons-material/Delete'
import EditIcon from '@mui/icons-material/Edit'
import AddIcon from '@mui/icons-material/Add'
import { useCallback, useEffect, useState } from 'react';
import AlertDialog from './AlertDialog';
import { enqueueSnackbar, useSnackbar } from 'notistack';
import { Proceso } from '../types/Proceso';
import { workflowPhaseService } from '../services/settings/workflowPhaseService';
import WorkflowPhaseAddEditDialog from '../dialogs/WorkflowPhaseAddEditDialog';
import { Fase, Transicion } from '../types/Fase';
import GroupIcon from '@mui/icons-material/Group';
import WorkflowPhaseUsersDialog from '../dialogs/WorkflowPhaseUsersDialog';
import WorkflowPhaseTransitionAddEditDialog from '../dialogs/WorkflowPhaseTransitionAddEditDialog';
import ForwardToInboxIcon from '@mui/icons-material/ForwardToInbox';
import ChecklistIcon from '@mui/icons-material/Checklist';
import WorkflowPhaseTransitionUsersDialog from '../dialogs/WorkflowPhaseTransitionUsersDialog';
import WorkflowPhaseTransitionEmailsDialog from '../dialogs/WorkflowPhaseTransitionEmailsDialog';
import WorkflowPhaseTransitionRequirementsDialog from '../dialogs/WorkflowPhaseTransitionRequirementsDialog';

function PhaseRow(
  props: { 
    phase: Fase, 
    selectedWorkflow: Proceso,
    phasesList: Fase[] | undefined,
    callbackAction: () => void,
    editAction: (phase: any) => void,
    usersAction: (phase: any) => void,
    deleteAction: (phase: any) => void
  }
) {

  const { phase, selectedWorkflow, phasesList, callbackAction, editAction, usersAction, deleteAction } = props;
  const [open, setOpen] = React.useState(false);
  const [openWorkflowPhaseTransitionAddEditDialog, setOpenWorkflowPhaseTransitionAddEditDialog] = useState<boolean>(false);
  const [openWorkflowPhaseTransitionDeleteDialog, setOpenWorkflowPhaseTransitionDeleteDialog] = useState<boolean>(false);
  const [selectedTransition, setSelectedTransition] = useState<Transicion>();
  const [openWorkflowPhaseTransitionUsersDialog, setOpenWorkflowPhaseTransitionUsersDialog] = useState<boolean>(false);
  const [openWorkflowPhaseTransitionRequirementsDialog, setOpenWorkflowPhaseTransitionRequirementsDialog] = useState<boolean>(false);
  const [openWorkflowPhaseTransitionEmailsDialog, setOpenWorkflowPhaseTransitionEmailsDialog] = useState<boolean>(false);

  // Phase transition dialog
  const handleNewTransitionClick = () => {
    setSelectedTransition(undefined);
    setOpenWorkflowPhaseTransitionAddEditDialog(true);
  }

  const handleCloseWorkflowPhaseTransitionAddEditDialog = () => {
    setOpenWorkflowPhaseTransitionAddEditDialog(false);
  }

  const handleCloseWorkflowPhaseTransitionAddEditDialogFromAction = async (actionResult: boolean) => {
    setOpenWorkflowPhaseTransitionAddEditDialog(false);
    
    if(actionResult) {
      callbackAction();
    }
  }

  const handleOpenWorkflowPhaseTransitionAddEditDialog = (data: any) => {    
    setSelectedTransition(data);
    setOpenWorkflowPhaseTransitionAddEditDialog(true);
  }

  // Delete phase transition dialog
  const handleCloseWorkflowPhaseTransitionDeleteDialog = () => {   
    setOpenWorkflowPhaseTransitionDeleteDialog(false);
  }
    
  const handleCloseWorkflowPhaseTransitionDeleteDialogFromAction = async (actionResult: boolean) => {
    try {
      if(actionResult && selectedTransition) {
        await workflowPhaseService.deletePhaseTransition(selectedTransition.idProceso, selectedTransition.idFaseOrigen, selectedTransition.idFaseDestino);
        enqueueSnackbar("Transición eliminada.", { variant: "success" });
        callbackAction();
      }
      setOpenWorkflowPhaseTransitionDeleteDialog(false);      
    }
    catch(error){
      enqueueSnackbar("Ocurrió un error al eliminar la transición.", { variant: "error" });
    }
  }

  const handleOpenWorkflowPhaseTransitionDeleteDialog = (data: any) => {    
    setSelectedTransition(data);
    setOpenWorkflowPhaseTransitionDeleteDialog(true);
  }

  // Transition Users Dialog

  const handleOpenWorkflowPhaseTransitionUsersDialog = (data: Transicion) => {        
    setSelectedTransition(data);    
    setOpenWorkflowPhaseTransitionUsersDialog(true);
  }

  const handleCloseWorkflowPhaseTransitionUsersDialog = () => {
    setOpenWorkflowPhaseTransitionUsersDialog(false);
  }

  const handleCloseWorkflowPhaseTransitionUsersDialogFromAction = async (actionResult: boolean) => {
    setOpenWorkflowPhaseTransitionUsersDialog(false);
    
    if(actionResult) {
      callbackAction();
    }
  }

  // Transition Users Dialog

  const handleOpenWorkflowPhaseTransitionRequirementsDialog = (data: Transicion) => {        
    setSelectedTransition(data);    
    setOpenWorkflowPhaseTransitionRequirementsDialog(true);
  }

  const handleCloseWorkflowPhaseTransitionRequirementsDialog = () => {
    setOpenWorkflowPhaseTransitionRequirementsDialog(false);
  }

  const handleCloseWorkflowPhaseTransitionRequirementsDialogFromAction = async (actionResult: boolean) => {
    setOpenWorkflowPhaseTransitionRequirementsDialog(false);
    
    if(actionResult) {
      callbackAction();
    }
  }


  // Transition Emails Dialog
  
  const handleOpenWorkflowPhaseTransitionEmailsDialog = (data: Transicion) => {        
    setSelectedTransition(data);    
    setOpenWorkflowPhaseTransitionEmailsDialog(true);
  }

  const handleCloseWorkflowPhaseTransitionEmailsDialog = () => {
    setOpenWorkflowPhaseTransitionEmailsDialog(false);
  }

  const handleCloseWorkflowPhaseTransitionEmailsDialogFromAction = async (actionResult: boolean) => {
    setOpenWorkflowPhaseTransitionEmailsDialog(false);
    
    if(actionResult) {
      callbackAction();
    }
  }

  return (
    <React.Fragment>
      <TableRow hover sx={{}}>
        <TableCell>
          <IconButton
            aria-label="expand row"
            size="small"
            onClick={() => setOpen(!open)}
          >
            {open ? <KeyboardArrowUpIcon /> : <KeyboardArrowDownIcon />}
          </IconButton>
        </TableCell>        
        <TableCell align={'left'}> 
          {phase.nombre}
        </TableCell>
        <TableCell align={'left'}> 
          {phase.tipoFase}
        </TableCell>
        <TableCell align={'left'}> 
          {phase.unidadAdministrativa}
        </TableCell>
        <TableCell align={'left'}> 
          <Checkbox checked={phase.asignacionObligatoria} disabled={true} />
        </TableCell>
        <TableCell align={'left'}> 
        <Checkbox checked={phase.activa} disabled={true} />
        </TableCell>
        <TableCell>
          <IconButton onClick={editAction}>
            <Tooltip title={'Editar'} placement="top" arrow>
              <EditIcon />
            </Tooltip>            
          </IconButton>
          <IconButton onClick={usersAction}>
            <Tooltip title={"Usuarios Asignados"} placement="top" arrow>
              <GroupIcon />
            </Tooltip>            
          </IconButton>
          <IconButton onClick={deleteAction}>
            <Tooltip title={"Eliminar"} placement="top" arrow>
              <DeleteIcon />
            </Tooltip>            
          </IconButton>
        </TableCell>
      </TableRow>

      {/* Contenido collapsable */}
      <TableRow>
        <TableCell style={{ paddingBottom: 0, paddingTop: 0 }} colSpan={7}>
          <Collapse in={open} timeout="auto" unmountOnExit>
            <Box sx={{ margin: "34px" }}>
              <Alert severity="info" icon={false}>
                <div style={{ marginTop: "8px" }}>
                  Transiciones de la fase: <b>{phase.nombre}</b>
                </div>
              </Alert>
              
              <Toolbar style={{ paddingLeft: "0px", justifyContent: "flex-end" }}>          
                <Button disableElevation onClick={() => { handleNewTransitionClick(); }}>
                  <AddIcon fontSize="small" /> Nueva Transición
                </Button>
              </Toolbar> 

              <Table stickyHeader size="small" component={Paper}>
                <TableHead>
                  <TableRow>
                    <TableCell>
                      <b>Fase Destino</b>
                    </TableCell>
                    <TableCell>
                      <b>Unidad Administrativa</b>
                    </TableCell>
                    <TableCell>
                      <b>Estado</b>
                    </TableCell>
                    <TableCell>
                      <b>Acciones</b>
                    </TableCell>
                  </TableRow>
                </TableHead>
                <TableBody>
                  {
                    phase.listaTransiciones !== null ? (
                      phase.listaTransiciones?.map((transition: any, index: number) => (
                          <TableRow key={index} hover>
                            <TableCell>
                              {transition.faseDestino}
                            </TableCell>
                            <TableCell>
                              {transition.unidadAdministrativaFD}
                            </TableCell>
                            <TableCell>
                              <Checkbox checked={transition.activa} disabled={true} />
                            </TableCell>
                            <TableCell>
                              <IconButton onClick={() => {handleOpenWorkflowPhaseTransitionAddEditDialog(transition)}}>
                                <Tooltip title={"Editar Transición"} placement="top" arrow>
                                  <EditIcon />
                                </Tooltip>                                
                              </IconButton>
                              <IconButton onClick={() => {handleOpenWorkflowPhaseTransitionUsersDialog(transition)}}>
                                <Tooltip title={"Usuarios Asignados"} placement="top" arrow>
                                  <GroupIcon />
                                </Tooltip>                                
                              </IconButton>
                              <IconButton onClick={() => {handleOpenWorkflowPhaseTransitionEmailsDialog(transition)}}>
                                <Tooltip title={"Notificaciones Adicionales"} placement="top" arrow>
                                  <ForwardToInboxIcon />
                                </Tooltip>                               
                              </IconButton>
                              <IconButton onClick={() => {handleOpenWorkflowPhaseTransitionRequirementsDialog(transition)}}>
                                <Tooltip title={"Requisitos por Transición"} placement="top" arrow>
                                  <ChecklistIcon />
                                </Tooltip>                                
                              </IconButton>
                              <IconButton onClick={() => {handleOpenWorkflowPhaseTransitionDeleteDialog(transition)}}>
                                <Tooltip title={"Eliminar"} placement="top" arrow>
                                  <DeleteIcon />
                                </Tooltip>                                
                              </IconButton>
                            </TableCell>
                          </TableRow>
                      ))
                    ) : (<></>)
                  }
                </TableBody>
              </Table>
            </Box>
          </Collapse>
        </TableCell>
      </TableRow>

      <Dialog
        open={openWorkflowPhaseTransitionAddEditDialog}
        onClose={handleCloseWorkflowPhaseTransitionAddEditDialog}
        maxWidth={'md'}
      >
        <WorkflowPhaseTransitionAddEditDialog
          selectedPhaseName = {phase.nombre}
          selectedWorkflow={selectedWorkflow}
          phasesList={phasesList}
          selectedTransition={selectedTransition}
          mode={!selectedTransition ? "add" : "edit"}
          onClose={handleCloseWorkflowPhaseTransitionAddEditDialogFromAction}
        />
      </Dialog>

      <Dialog
        open={openWorkflowPhaseTransitionUsersDialog}
        onClose={handleCloseWorkflowPhaseTransitionUsersDialog}
        maxWidth={'md'}
      >
        <WorkflowPhaseTransitionUsersDialog  
          selectedWorkflow={selectedWorkflow}
          selectedTransition={selectedTransition}
          onClose={handleCloseWorkflowPhaseTransitionUsersDialogFromAction}
        />
      </Dialog>

      <Dialog
        open={openWorkflowPhaseTransitionEmailsDialog}
        onClose={handleCloseWorkflowPhaseTransitionEmailsDialog}
        maxWidth={'md'}
      >
        <WorkflowPhaseTransitionEmailsDialog  
          selectedWorkflow={selectedWorkflow}
          selectedTransition={selectedTransition}
          onClose={handleCloseWorkflowPhaseTransitionEmailsDialogFromAction}
        />
      </Dialog>

      <Dialog
        open={openWorkflowPhaseTransitionRequirementsDialog}
        onClose={handleCloseWorkflowPhaseTransitionRequirementsDialog}
        maxWidth={'md'}
      >
        <WorkflowPhaseTransitionRequirementsDialog  
          selectedWorkflow={selectedWorkflow}
          selectedTransition={selectedTransition}
          onClose={handleCloseWorkflowPhaseTransitionRequirementsDialogFromAction}
        />
      </Dialog>

      <Dialog
        open={openWorkflowPhaseTransitionDeleteDialog}
        onClose={handleCloseWorkflowPhaseTransitionDeleteDialog}
        maxWidth={"sm"}
      >
        <AlertDialog
          color={"error"}
          title={"Eliminar transición"}
          message={
            <>
              ¿Está seguro que desea eliminar la transición: <b>{selectedTransition?.faseOrigen + ' -> ' + selectedTransition?.faseDestino}</b>? Esta acción no se puede deshacer.
            </>
          }
          onClose={handleCloseWorkflowPhaseTransitionDeleteDialogFromAction}
        />
      </Dialog> 

    </React.Fragment>
  );
}

export type TableColumnType = {
  id: string;
  label: string;
  minWidth?: number;
  align?: 'left' | 'center' | 'right';
  format?: (value: number) => string;
}

export type TableRowsType<T> = T[][];

type TableProps = {
  selectedWorkflow: Proceso;
}

export default function WofkflowPhasesList({
  selectedWorkflow
}: TableProps) {
  
  const { enqueueSnackbar } = useSnackbar();
  const [loading, setLoading] = useState<boolean>(false);

  const [rows, setRows] = useState<TableColumnType[]>();
  const [selectedPhase, setSelectedPhase] = useState<Fase>();
  const [phasesList, setPhasesList] = useState<Fase[]>();

  const [openWorkflowPhaseAddEditDialog, setOpenWorkflowPhaseAddEditDialog] = useState<boolean>(false);
  const [openWorkflowPhaseUsersDialog, setOpenWorkflowPhaseUsersDialog] = useState<boolean>(false);
  const [openWorkflowPhaseDeleteDialog, setOpenWorkflowPhaseDeleteDialog] = useState<boolean>(false);
  
  // Phase
  const handleCloseWorkflowPhaseAddEditDialog = () => {
    setOpenWorkflowPhaseAddEditDialog(false);
  }

  const handleCloseWorkflowPhaseAddEditDialogFromAction= async (actionResult: boolean = false) => {
    if(actionResult) {
      fetchPhases(selectedWorkflow.idProceso);
    }
    setOpenWorkflowPhaseAddEditDialog(false);
  }

  const handleNewPhaseClick = () => {
    setSelectedPhase(undefined);
    setOpenWorkflowPhaseAddEditDialog(true);
  }

  const handleOpenPhaseAddEditDialog = (phase: any) => {       

    setSelectedPhase(phase);
    setOpenWorkflowPhaseAddEditDialog(true);
  };

  // Delete phase dialog
  const handleCloseWorkflowPhaseDeleteDialog = () => {   
    setOpenWorkflowPhaseDeleteDialog(false);
  }
    
  const handleCloseWorkflowPhaseDeleteDialogFromAction = async (actionResult: boolean) => {
    try {
      if(actionResult && selectedPhase) {
        await workflowPhaseService.delete(selectedPhase);
        enqueueSnackbar("Fase eliminada.", { variant: "success" });
        fetchPhases(selectedWorkflow.idProceso);      
      }
      setOpenWorkflowPhaseDeleteDialog(false);      
    }
    catch(error){
      enqueueSnackbar("Ocurrió un error al eliminar la fase.", { variant: "error" });
    }
  }

  const handleOpenWorkflowPhaseDeleteDialog = (phase: any) => {
    setSelectedPhase(phase);
    setOpenWorkflowPhaseDeleteDialog(true);
  }

  // Phase dialog
  const handleCloseWorkflowPhaseUsersDialog = () => {
    setOpenWorkflowPhaseUsersDialog(false);
  }

  const handleCloseWorkflowPhaseUsersDialogFromAction = async (actionResult: boolean) => {
    if(actionResult) {
      fetchPhases(selectedWorkflow.idProceso);
    }
    setOpenWorkflowPhaseUsersDialog(false);
  }

  const handleOpenWorkflowPhaseUsersDialog = (phase: any) => {
    setSelectedPhase(phase);
    setOpenWorkflowPhaseUsersDialog(true);
  }

  // Fetch phases
  const fetchPhases = useCallback(async (workflowId: number) => {
      try {

        setLoading(true);

        const rowsTemp: any[] = [];
        const response = await workflowPhaseService.getAll(workflowId);

        if (response.statusText === "OK") {

          response.data.forEach((item: any) => {
            rowsTemp.push([
              item.idFase,
              item.nombre,
              item.tipoFase,
              item.unidadAdministrativa,
              item.asignacionObligatoria,
              item.activa,
              item.listaTransiciones
            ]);
          });

          setPhasesList([...response.data]);
          setRows(rowsTemp);
          setLoading(false);

        } else {
          enqueueSnackbar("Ocurrió un error al obtener la lista de fases.", {
            variant: "error",
          });
        }
      } catch (error: any) {
        enqueueSnackbar(
          "Ocurrió un error al obtener la lista de fases. Detalles: " +
            error.message,
          { variant: "error" }
        );
        setLoading(false);
      }
    },
    [enqueueSnackbar]
  );


  useEffect(() => {

    fetchPhases(selectedWorkflow.idProceso);

  }, [fetchPhases, selectedWorkflow]); 

  return (
    <>
      <Toolbar style={{ paddingLeft: "0px", justifyContent: "flex-end" }}>          
        <Button disableElevation onClick={() => { handleNewPhaseClick(); }}>
          <AddIcon fontSize="small" /> Nueva Fase
        </Button>
      </Toolbar> 

      <TableContainer component={Paper}>
        <Table aria-label="collapsible table" size="small" stickyHeader>
          <TableHead>
            <TableRow>              
              <TableCell align={'left'} style={{ minWidth: 8 }}><b>{''}</b></TableCell>
              <TableCell align={'left'} style={{ minWidth: 100 }}><b>{'Nombre'}</b></TableCell>
              <TableCell align={'left'} style={{ minWidth: 100 }}><b>{'Tipo'}</b></TableCell>
              <TableCell align={'left'} style={{ minWidth: 100 }}><b>{'Unidad Administrativa'}</b></TableCell>
              <TableCell align={'left'} style={{ minWidth: 100 }}><b>{'Asignación Obligatoria'}</b></TableCell>
              <TableCell align={'left'} style={{ minWidth: 100 }}><b>{'Activa'}</b></TableCell>
              <TableCell align={'left'} style={{ minWidth: 100 }}><b>{'Acciones'}</b></TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {phasesList?.map((phase, index) => (
              <PhaseRow 
                key={index} 
                phase={phase}                 
                selectedWorkflow={selectedWorkflow}
                phasesList={phasesList}
                callbackAction={() => {fetchPhases(selectedWorkflow.idProceso)}}
                editAction={() => {handleOpenPhaseAddEditDialog(phase)}}
                usersAction={() => {handleOpenWorkflowPhaseUsersDialog(phase)}}
                deleteAction={() => {handleOpenWorkflowPhaseDeleteDialog(phase)}}              
              />
            ))}
          </TableBody>
        </Table>
      </TableContainer>

      <Dialog
        open={openWorkflowPhaseAddEditDialog}
        onClose={handleCloseWorkflowPhaseAddEditDialog}
        maxWidth={'md'}
      >
        <WorkflowPhaseAddEditDialog
          selectedPhase = {selectedPhase}
          selectedWorkflow={selectedWorkflow}
          mode={!selectedPhase ? "add" : "edit"}          
          onClose={handleCloseWorkflowPhaseAddEditDialogFromAction}
        />
      </Dialog>

      <Dialog
        open={openWorkflowPhaseUsersDialog}
        onClose={handleCloseWorkflowPhaseUsersDialog}
        maxWidth={'md'}
      >
        <WorkflowPhaseUsersDialog
          selectedPhase = {selectedPhase}
          selectedWorkflow={selectedWorkflow}
          onClose={handleCloseWorkflowPhaseUsersDialogFromAction}
        />
      </Dialog>

      <Dialog
        open={openWorkflowPhaseDeleteDialog}
        onClose={handleCloseWorkflowPhaseDeleteDialog}
        maxWidth={"sm"}
      >
        <AlertDialog
          color={"error"}
          title={"Eliminar fase"}
          message={
            <>
              ¿Está seguro que desea eliminar la fase: <b>{selectedPhase?.nombre}</b>? Esta acción no se puede deshacer.
            </>
          }
          onClose={handleCloseWorkflowPhaseDeleteDialogFromAction}
        />
      </Dialog>     

    </>
  );
}