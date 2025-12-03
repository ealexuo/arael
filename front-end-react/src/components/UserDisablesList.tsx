import Table from '@mui/material/Table';
import TableBody from '@mui/material/TableBody';
import TableCell from '@mui/material/TableCell';
import TableContainer from '@mui/material/TableContainer';
import TableHead from '@mui/material/TableHead';
import TableRow from '@mui/material/TableRow';
import Paper from '@mui/material/Paper';
import {Button, Dialog, IconButton, Toolbar, Tooltip } from '@mui/material';
import DeleteIcon from '@mui/icons-material/Delete'
import EditIcon from '@mui/icons-material/Edit'
import AddIcon from '@mui/icons-material/Add'
import { useCallback, useEffect, useState } from 'react';
import AlertDialog from './AlertDialog';
import { useSnackbar } from 'notistack';
import { Usuario } from '../types/Usuario';
import { userDisableService } from '../services/settings/userDisableService';
import UserDisableAddEditDialog from '../dialogs/UserDisableAddEditDialog';
import { Inhabilitacion } from '../types/Inhabilitacion';
import dayjs from 'dayjs'

export type TableColumnType = {
  id: string;
  label: string;
  minWidth?: number;
  align?: 'left' | 'center' | 'right';
  format?: (value: number) => string;
}

export type TableRowsType<T> = T[][];

type TableProps = {
  selectedUser: Usuario;
}

export default function UserDisablesList({
  selectedUser
}: TableProps) {

  const { enqueueSnackbar } = useSnackbar();
  const [loading, setLoading] = useState<boolean>(false);
  const [rows, setRows] = useState<TableColumnType[]>();  
  const [selectedDiable, setSelectedDiable] = useState<Inhabilitacion>();
  const [DiablesList, setDiablesList] = useState<Inhabilitacion[]>();  

  const [openWorkflowRequirementAddEditDialog, setOpenWorkflowRequirementAddEditDialog] = useState<boolean>(false);
  const [openWorkflowRequirementDeleteDialog, setOpenWorkflowRequirementDeleteDialog] = useState<boolean>(false);

  const handleNewRequirementClick = (inhabilitacion: Inhabilitacion | null) => {
    if (inhabilitacion)
      setSelectedDiable(inhabilitacion);
    else
      setSelectedDiable(undefined);
    setOpenWorkflowRequirementAddEditDialog(true);
  }

  // Add or Edit Requirement
  const handleCloseWorkflowRequirementsAddEditDialog = () => {
    setOpenWorkflowRequirementAddEditDialog(false);
  }

  const handleCloseWorkflowRequirementesAddEditDialogFromAction = async (actionResult: boolean = false) => {
    if (actionResult) {
      fetchRequitements(selectedUser.idUsuario);// se actualiza el listado
    }
    setOpenWorkflowRequirementAddEditDialog(false);
  }

  // Delete requirement dialog

  const handleOpenWorkflowRequirementTransitionDeleteDialog = (data: any) => {
    setSelectedDiable(data);
    setOpenWorkflowRequirementDeleteDialog(true);
  }


  const handleCloseWorkflowRequirementDeleteDialog = () => {
    setOpenWorkflowRequirementDeleteDialog(false);
  }

  const handleCloseWorkflowRequirementDeleteDialogFromAction = async (actionResult: boolean) => {
    try {      
      if (actionResult && selectedDiable) {
        await userDisableService.delete(selectedDiable);
        enqueueSnackbar("Inhabilitación eliminada.", { variant: "success" });
        fetchRequitements(selectedUser.idUsuario);
      }
      setOpenWorkflowRequirementDeleteDialog(false);
    }
    catch (error) {
      enqueueSnackbar("Ocurrió un error al eliminar la fase.", { variant: "error" });
    }
  }

  // Fetch Requirements
  const fetchRequitements = useCallback(async (userId: number) => {
    try {

      setLoading(true);

      const rowsTemp: any[] = [];
      const response = await userDisableService.getAll(userId);

      if (response.statusText === "OK") {

        response.data.forEach((item: any) => {
          rowsTemp.push([
            item.idEntidad,
            item.idUsuario,
            item.idHistoricoInhabilitacion,
            item.fechaInicio,
            item.fechaFin
          ]);
        });

        setDiablesList([...response.data]);
        setRows(rowsTemp);
        setLoading(false);

      } else {
        enqueueSnackbar("Ocurrió un error al obtener la lista de inhabilitaciones.", {
          variant: "error",
        });
      }
    } catch (error: any) {
      enqueueSnackbar(
        "Ocurrió un error al obtener la lista de inhabilitaciones. Detalles: " +
        error.message,
        { variant: "error" }
      );
      setLoading(false);
    }
  },
    [enqueueSnackbar]
  );


  useEffect(() => {

    fetchRequitements(selectedUser.idUsuario);

  }, [fetchRequitements, selectedUser]);

  return (
    <>
      <Toolbar style={{ paddingLeft: "0px", justifyContent: "flex-end" }}>
        <Button disableElevation onClick={() => { handleNewRequirementClick(null); }}>
          <AddIcon fontSize="small" /> Nueva Inhabilitación
        </Button>
      </Toolbar>

      <TableContainer component={Paper}>
        <Table aria-label="collapsible table" size="small" stickyHeader>
          <TableHead>
            <TableRow>
              <TableCell align={'left'} style={{ minWidth: 100 }}><b>{'Fecha Inicial'}</b></TableCell>
              <TableCell align={'left'} style={{ minWidth: 100 }}><b>{'Fecha Final'}</b></TableCell>
              <TableCell align={'left'} style={{ minWidth: 100 }}><b>{'Acciones'}</b></TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {
              DiablesList !== null ? (
                DiablesList?.map((inhabilitacion: any, index: number) => (
                  <TableRow key={index} hover>
                    <TableCell>
                      {inhabilitacion.fechaInicio
                        ? dayjs(inhabilitacion.fechaInicio).format('DD/MM/YYYY')
                        : ''}
                    </TableCell>
                    <TableCell>
                      {inhabilitacion.fechaFin
                        ? dayjs(inhabilitacion.fechaFin).format('DD/MM/YYYY')
                        : ''}
                    </TableCell>
                    <TableCell>
                      <IconButton onClick={() => { handleNewRequirementClick(inhabilitacion) }}>
                        <Tooltip title={"Editar Inhabilitación"} placement="top" arrow>
                          <EditIcon />
                        </Tooltip>
                      </IconButton>
                      <IconButton onClick={() => { handleOpenWorkflowRequirementTransitionDeleteDialog(inhabilitacion) }}>
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
        maxWidth={'sm'}
      >
        <UserDisableAddEditDialog
          selectedUser={selectedUser}
          selectedDisable={selectedDiable}
          mode={!selectedDiable ? "add" : "edit"}
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
              ¿Está seguro que desea eliminar la inhabilitación comprendida del:&nbsp; 
              <b>{dayjs(selectedDiable?.fechaInicio).format('DD/MM/YYYY')}</b> al&nbsp; 
              <b>{dayjs(selectedDiable?.fechaFin).format('DD/MM/YYYY')}</b>? Esta acción no se puede deshacer.
            </>
          }
          onClose={handleCloseWorkflowRequirementDeleteDialogFromAction}
        />
      </Dialog>

    </>
  );
}