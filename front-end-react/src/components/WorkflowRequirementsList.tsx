import * as React from 'react';

import Table from '@mui/material/Table';
import TableBody from '@mui/material/TableBody';
import TableCell from '@mui/material/TableCell';
import TableContainer from '@mui/material/TableContainer';
import TableHead from '@mui/material/TableHead';
import TableRow from '@mui/material/TableRow';
import Paper from '@mui/material/Paper';
import { Alert, Button, Checkbox, Dialog, IconButton, Toolbar, Tooltip } from '@mui/material';
import DeleteIcon from '@mui/icons-material/Delete'
import EditIcon from '@mui/icons-material/Edit'
import AddIcon from '@mui/icons-material/Add'
import { useCallback, useEffect, useState } from 'react';
import AlertDialog from './AlertDialog';
import { enqueueSnackbar, useSnackbar } from 'notistack';
import { Proceso } from '../types/Proceso';
import { Requisito } from '../types/Requisito';
import { workflowRequirementService } from '../services/settings/workflowRequirementService';
import WorkflowRequirementAddEditDialog from '../dialogs/WorkflowRequirementAddEditDialog';

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

export default function WorkflowRequirementsList({
  selectedWorkflow
}: TableProps) {

  const { enqueueSnackbar } = useSnackbar();
  const [loading, setLoading] = useState<boolean>(false);

  const [rows, setRows] = useState<TableColumnType[]>();
  const [selectedRequirement, setSelectedRequirement] = useState<Requisito>();
  const [RequirementsList, setRequirementsList] = useState<Requisito[]>();

  const [openWorkflowRequirementAddEditDialog, setOpenWorkflowRequirementAddEditDialog] = useState<boolean>(false);
  const [openWorkflowRequirementDeleteDialog, setOpenWorkflowRequirementDeleteDialog] = useState<boolean>(false);

  const handleNewRequirementClick = (requisito: Requisito | null) => {
    if (requisito)
      setSelectedRequirement(requisito);
    else
      setSelectedRequirement(undefined);
    setOpenWorkflowRequirementAddEditDialog(true);
  }

  // Add or Edit Requirement
  const handleCloseWorkflowRequirementsAddEditDialog = () => {
    setOpenWorkflowRequirementAddEditDialog(false);
  }

  const handleCloseWorkflowRequirementesAddEditDialogFromAction = async (actionResult: boolean = false) => {
    if (actionResult) {
      fetchRequitements(selectedWorkflow.idProceso);// se actualiza el listado
    }
    setOpenWorkflowRequirementAddEditDialog(false);
  }

  // Delete requirement dialog

  const handleOpenWorkflowRequirementTransitionDeleteDialog = (data: any) => {
    setSelectedRequirement(data);
    setOpenWorkflowRequirementDeleteDialog(true);
  }


  const handleCloseWorkflowRequirementDeleteDialog = () => {
    setOpenWorkflowRequirementDeleteDialog(false);
  }

  const handleCloseWorkflowRequirementDeleteDialogFromAction = async (actionResult: boolean) => {
    try {
      if (actionResult && selectedRequirement) {
        await workflowRequirementService.delete(selectedRequirement);
        enqueueSnackbar("Fase eliminada.", { variant: "success" });
        fetchRequitements(selectedWorkflow.idProceso);
      }
      setOpenWorkflowRequirementDeleteDialog(false);
    }
    catch (error) {
      enqueueSnackbar("Ocurrió un error al eliminar la fase.", { variant: "error" });
    }
  }

  // Fetch Requirements
  const fetchRequitements = useCallback(async (workflowId: number) => {
    try {

      setLoading(true);

      const rowsTemp: any[] = [];
      const response = await workflowRequirementService.getAll(workflowId);

      if (response.statusText === "OK") {

        response.data.forEach((item: any) => {
          rowsTemp.push([
            item.idEntidad,
            item.idProceso,
            item.idRequisito,
            item.requisito,
            item.obligatorio
          ]);
        });

        setRequirementsList([...response.data]);
        setRows(rowsTemp);
        setLoading(false);

      } else {
        enqueueSnackbar("Ocurrió un error al obtener la lista de requisitos.", {
          variant: "error",
        });
      }
    } catch (error: any) {
      enqueueSnackbar(
        "Ocurrió un error al obtener la lista de requisitos. Detalles: " +
        error.message,
        { variant: "error" }
      );
      setLoading(false);
    }
  },
    [enqueueSnackbar]
  );


  useEffect(() => {

    fetchRequitements(selectedWorkflow.idProceso);

  }, [fetchRequitements, selectedWorkflow]);

  return (
    <>
      <Toolbar style={{ paddingLeft: "0px", justifyContent: "flex-end" }}>
        <Button disableElevation onClick={() => { handleNewRequirementClick(null); }}>
          <AddIcon fontSize="small" /> Nuevo Requisito
        </Button>
      </Toolbar>

      <TableContainer component={Paper}>
        <Table aria-label="collapsible table" size="small" stickyHeader>
          <TableHead>
            <TableRow>
              <TableCell align={'left'} style={{ minWidth: 100 }}><b>{'Requisito'}</b></TableCell>
              <TableCell align={'left'} style={{ minWidth: 100 }}><b>{'Obligatorio'}</b></TableCell>
              <TableCell align={'left'} style={{ minWidth: 100 }}><b>{'Acciones'}</b></TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {
              RequirementsList !== null ? (
                RequirementsList?.map((requisito: any, index: number) => (
                  <TableRow key={index} hover>
                    <TableCell>
                      {requisito.requisito}
                    </TableCell>
                    <TableCell>
                      <Checkbox checked={requisito.obligatorio} disabled={true} />
                    </TableCell>
                    <TableCell>
                      <IconButton onClick={() => { handleNewRequirementClick(requisito) }}>
                        <Tooltip title={"Editar Requisito"} placement="top" arrow>
                          <EditIcon />
                        </Tooltip>
                      </IconButton>
                      <IconButton onClick={() => { handleOpenWorkflowRequirementTransitionDeleteDialog(requisito) }}>
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
      </TableContainer>

      <Dialog
        open={openWorkflowRequirementAddEditDialog}
        onClose={handleCloseWorkflowRequirementsAddEditDialog}
        maxWidth={'md'}
      >
        <WorkflowRequirementAddEditDialog
          selectedRequirement={selectedRequirement}
          selectedWorkflow={selectedWorkflow}
          mode={!selectedRequirement ? "add" : "edit"}
          onClose={handleCloseWorkflowRequirementesAddEditDialogFromAction}
        />
      </Dialog>

      <Dialog
        open={openWorkflowRequirementDeleteDialog}
        onClose={handleCloseWorkflowRequirementDeleteDialog}
        maxWidth={"sm"}
      >
        <AlertDialog
          color={"error"}
          title={"Eliminar requisito"}
          message={
            <>
              ¿Está seguro que desea eliminar el requisito: <b>{selectedRequirement?.requisito}</b>? Esta acción no se puede deshacer.
            </>
          }
          onClose={handleCloseWorkflowRequirementDeleteDialogFromAction}
        />
      </Dialog>

    </>
  );
}