import React, { useCallback, useEffect, useState } from 'react'
import {TableColumnType, StickyHeadTable, ItemActionListType} from '../../components/StickyHeadTable'
import Page from '../../components/Page'
import { userService } from '../../services/settings/userService';
import { administrativeUnitsService } from '../../services/settings/administrativeUnitsService';
import { processPermissionService } from '../../services/settings/processPermissionService';
import Loader from '../../components/Loader';
import UserAddEditDialog from '../../dialogs/UserAddEditDialog';
import EditIcon from '@mui/icons-material/Edit';
import CalendarMonthIcon from '@mui/icons-material/CalendarMonth';
import BadgeIcon from '@mui/icons-material/Badge'
import DeleteIcon from '@mui/icons-material/Delete'
import Dialog from '@mui/material/Dialog';
import { useSnackbar } from 'notistack';
import UserDisableDialog from '../../dialogs/UserDisableDialog';
import moment from 'moment';
import AlertDialog from '../../components/AlertDialog';
import ProcessPermissionDialog from '../../dialogs/ProcessPermissionDialog';

const columnsInit: TableColumnType[] = [
  { 
    id: "userId", 
    label: 'Id', 
    minWidth: 5,
  },
  { 
    id: "personalId", 
    label: 'No Identifación Personal', 
    minWidth: 50 
  },
  { 
    id: "name", 
    label: "Nombre", 
    minWidth: 100 
  },
  {
    id: "email",
    label: "Correo Electrónico",
    minWidth: 100,    
  },
  // {
  //   id: "administrativeUnit",
  //   label: "Unidad Administrativa",
  //   minWidth: 100,    
  // },
  {
    id: "isActive",
    label: "Estado",
    minWidth: 100,    
  },
  // {
  //   id: "disableDates",
  //   label: "Fechas Inhabilitación",
  //   minWidth: 100,    
  // },
  {
    id: "actions",
    label: "Acciones",
    minWidth: 100,    
  }
];

const emptyUserObject = {
  idUsuario: -1,
  idEntidad: 0,
  idIdioma: 0,
  genero: 0,
  titulo: '',
  noIdentificacionPersonal: '',
  correoElectronico: '',
  primerNombre: '',
  segundoNombre: '',
  otrosNombres: '',
  primerApellido: '',
  segundoApellido: '',
  apellidoCasada: '',
  unidadAdministrativa: '',
  cargo: '',
  extension: '',
  telefono: '',
  activo: false,  
}

export default function Users() {

  const { enqueueSnackbar } = useSnackbar();

  const [loading, setLoading] = useState<boolean>(false);
  const [columns, setColumns] = useState(columnsInit as any);
  const [rows, setRows] = useState([] as any);
  const [searchText, setSearchText] = useState<string>('');
  
  const [totalRows, setTotalRows] = useState<number>(10);
  const [rowsPerPage, setRowsPerPage] = useState<number>(10);
  const [currentPage, setCurrentPage] = useState<number>(0);

  const [openUserAddEditDialog, setOpenUserAddEditDialog] = useState<boolean>(false);
  const [openUserDisableDialog, setOpenUserDisableDialog] = useState<boolean>(false);
  const [openProcessPermissionDialog, setOpenProcessPermissionDialog] = useState<boolean>(false);
  
  const [openUserDeleteDialog, setOpenUserDeleteDialog] = useState<boolean>(false);

  const [selectedUser, setSelectedUser] = useState<any>(null);
  const [administrativeUnitslist, setAdministrativeUnitslist] = useState<any>(null);
  const [processPermissionList, setProcessPermissionList] = useState<any>(null);

  /** Fetch Data Section */

  const fetchUsers = useCallback(async (initialPage: number, usersPerPage: number, searchString: string) => {
    try {
      setLoading(true);
      
      const rowsTemp: any[] = [];
      const response = await userService.getAll(initialPage + 1, usersPerPage, searchString);

      if(response.statusText === 'OK') {
        if(response.data.cantidadTotal){
          setTotalRows(response.data.cantidadTotal);
        }

        response.data.listaUsuarios.forEach((item: any) => {
          rowsTemp.push([
            item.idUsuario,
            item.noIdentificacionPersonal,
            item.nombre,
            item.correoElectronico,
            //item.nombreUnidadAdministrativa,
            item.estado,
            //item.fechasInhabilitacion === null ? '' : moment(item.fechasInhabilitacion.fechaInicio).format('L') + ' -> ' + moment(item.fechasInhabilitacion.fechaFin).format('L') 
          ]);
        });

        setRows(rowsTemp);
        setLoading(false);  
      }
      else {
        enqueueSnackbar('Ocurrió un error al obtener la lista de usuarios.', { variant: 'error' });
      }        
    }
    catch(error: any){
      enqueueSnackbar('Ocurrió un error al obtener la lista de usuarios. Detalles: ' + error.message, { variant: 'error' });
      setLoading(false);
    }
  }, [enqueueSnackbar]);

  const fetchUser = async (userId: number) =>{
    
    try {
      const response = await userService.get(userId);
      if(response.statusText === 'OK') {
        setLoading(false);        
        return response.data;
      }
      enqueueSnackbar('Error al obtener usuario.', { variant: 'error' });
    }
    catch{
      enqueueSnackbar('Error al obtener usuario.', { variant: 'error' });
      setLoading(false);
    }
    
    return null;
  };   

  const fetchProcessPermissions = async (userId: number) =>{
    
    setLoading(true);

    try {
      const response = await processPermissionService.get(userId);
      if(response.statusText === 'OK') {
        setLoading(false);        
        setProcessPermissionList(response.data);
      }
      else {
        enqueueSnackbar('Error al obtener la lista de permisos.', { variant: 'error' });
      }      
    }
    catch{
      enqueueSnackbar('Error al obtener la lista de permisos.', { variant: 'error' });
      setLoading(false);
    }
    
  };

  const deleteSelectedUser = async (entityId: number, userId: number) => {

    setLoading(true);

    try {
      const response = await userService.delete(entityId, userId); 

      if (response.statusText === "OK") {
        setLoading(false);
        enqueueSnackbar('Usuario eliminado.', { variant: "success" });
      } else {
        enqueueSnackbar('Ocurrió un Error al eliminar al usuario.', { variant: "error" });
      }
    } catch (error: any) {
      enqueueSnackbar('Ocurrió un Error al eliminar al usuario.. Detalles: ' + error.message, { variant: "error" });
      setLoading(false);
    }

  }
  
  const fetchAdministrativeUnits = useCallback(async () =>{ 

    try {
      const response = await administrativeUnitsService.getAll();
      if(response.statusText === 'OK') {
        setLoading(false);        
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
      setLoading(false);
    }
    
    return null;    
  }, [enqueueSnackbar]); 

  const fetchAdministrativeUnit = async (administrativeUnitId: number) =>{ 

    try {
      const response = await administrativeUnitsService.get(administrativeUnitId);
      if(response.statusText === 'OK') {
        setLoading(false);        
        return response.data;
      }
      enqueueSnackbar('Error al obtener unidad administrativa.', { variant: 'error' });
    }
    catch{
      enqueueSnackbar('Error al obtener unidad administrativa.', { variant: 'error' });
    }
    
    return null;    
  }; 

  /** Handle Functions Section */

  const handlePageChange = (event: unknown, newPage: number) => {
    setCurrentPage(newPage);
  };

  const handleRowsPerPageChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setRowsPerPage(+event.target.value);
    setCurrentPage(0);
  };

  const handleSearchTextChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setSearchText(event.target.value);
  };

  // User Add/Edit dialog
  const handleOpenUserAddEditDialog = () => {
    setSelectedUser(emptyUserObject);
    setOpenUserAddEditDialog(true);
  }

  const handleCloseUserAddEditDialog = () => {
    setOpenUserAddEditDialog(false);
  }

  const handleCloseUserAddEditDialogFromAction = (refreshUsersList: boolean = false) => {
    if(refreshUsersList) {
      fetchUsers(currentPage, rowsPerPage, searchText);
    }
    setOpenUserAddEditDialog(false);
  }

  const handleSelectedUserEdit = async (user: any) => {
    const userData = await fetchUser(user && user[0] ? user[0] : 0);
    const administrativeUnit = await fetchAdministrativeUnit(userData.idUnidadAdministrativa);

    userData.unidadAdministrativa = administrativeUnit.nombre;
        
    setSelectedUser(userData);
    setOpenUserAddEditDialog(true);
  }

  // User Disable dialog
  const handleOpenUserDisableDialog = async (user: any) => {
    const userData = await fetchUser(user && user[0] ? user[0] : 0);
      
    setSelectedUser(userData);
    setOpenUserDisableDialog(true);
  }

  const handleCloseUserDisableDialog = () => {
    setOpenUserDisableDialog(false);
  }

  const handleCloseUserDisableDialogFromAction = (refreshUsersList: boolean = false) => {
    if(refreshUsersList) {
      fetchUsers(currentPage, rowsPerPage, searchText);
    }
    setOpenUserDisableDialog(false);
  }
  
  // Process Permission dialog
  const handleOpenProcessPermissionDialog = async (user: any) => {
    const userData = await fetchUser(user && user[0] ? user[0] : 0);
    await fetchProcessPermissions(userData.idUsuario);

    setSelectedUser(userData);
    setOpenProcessPermissionDialog(true);
  }

  const handleCloseProcessPermissionDialog = () => {
    setOpenProcessPermissionDialog(false);
  }

  const handleCloseProcessPermissionDialogFromAction = (actionResult: boolean = false) => {
    if(actionResult) {
      fetchUsers(currentPage, rowsPerPage, searchText);
    }
    setOpenProcessPermissionDialog(false);
  }
    

  // User Delete Alert dialog
  const handleOpenUserDeleteDialog = async (user: any) => {
    const userData = await fetchUser(user && user[0] ? user[0] : 0);
      
    setSelectedUser(userData);
    setOpenUserDeleteDialog(true);
  }

  const handleCloseUserDeleteDialog = () => {    
    setOpenUserDeleteDialog(false);
  }

  const handleCloseUserDeleteDialogFromAction = async (actionResult: boolean = false) => {
    if(actionResult) {
      await deleteSelectedUser(selectedUser.idEntidad, selectedUser.idUsuario);
      await fetchUsers(currentPage, rowsPerPage, searchText);
    }
    setOpenUserDeleteDialog(false);
  } 


  /** Defined Objects Section */
  const actionList: ItemActionListType =
  [
    { 
      name: 'edit',
      icon: <EditIcon />,
      callBack: handleSelectedUserEdit, 
    },
    { 
      name: 'disable',
      icon: <CalendarMonthIcon />,
      callBack: handleOpenUserDisableDialog, 
    },
    { 
      name: 'roles',
      icon: <BadgeIcon />,
      callBack: handleOpenProcessPermissionDialog, 
    },
    { 
      name: 'delete',
      icon: <DeleteIcon />,
      callBack: handleOpenUserDeleteDialog, 
    }
  ]; 

  /** Use Effect Section */

  useEffect(() => {

    setColumns(columnsInit);
    fetchUsers(currentPage, rowsPerPage, searchText);
    fetchAdministrativeUnits(); 

  }, [currentPage, rowsPerPage, searchText, fetchUsers, fetchAdministrativeUnits]);

  /** Return Section */
  return (
    <>
      <Page title="Usuarios">
        {
          loading ? (
            <Loader />
          ) : (
            <StickyHeadTable
              columns={columns}
              rows={rows}
              addActionRoute={"/settings/users/add-user"}
              addACtionToolTip="Nuevo Usuario"
              currentPage={currentPage}
              rowsPerPage={rowsPerPage}
              totalRows={totalRows}
              onPageChange={handlePageChange}
              onRowsPerPageChange={handleRowsPerPageChange}
              onSearchTextChange={handleSearchTextChange}
              onAddActionClick={handleOpenUserAddEditDialog}
              itemActionList={actionList}
            ></StickyHeadTable>
          )
        }
      </Page>

      <Dialog
        open={openUserAddEditDialog}
        onClose={handleCloseUserAddEditDialog}
        maxWidth={"lg"}        
      >
        <UserAddEditDialog 
          mode = {selectedUser && selectedUser.idUsuario > -1 ? 'edit' : 'add'}
          selectedUser = {selectedUser}
          administrativeUnitsList = {administrativeUnitslist}
          onClose = {handleCloseUserAddEditDialogFromAction}
        />        
      </Dialog>

      <Dialog
        open={openUserDisableDialog}
        onClose={handleCloseUserDisableDialog}
        maxWidth={"md"}
      >
        <UserDisableDialog 
          mode = {selectedUser && selectedUser.idUsuario > -1 ? 'edit' : 'add'}
          selectedUser = {selectedUser}          
          onClose = {handleCloseUserDisableDialogFromAction}
        />        
      </Dialog>

      <Dialog
        open={openProcessPermissionDialog}
        onClose={handleCloseProcessPermissionDialog}
        maxWidth={"md"}
      >
        <ProcessPermissionDialog 
          selectedUser = {selectedUser}
          processPermissionList = {processPermissionList}
          onClose = {handleCloseProcessPermissionDialogFromAction}
        />        
      </Dialog>

      <Dialog
        open={openUserDeleteDialog}
        onClose={handleCloseUserDeleteDialog}
        maxWidth={"sm"}
      >
        <AlertDialog
          color = {'error'}
          title = {'Eliminar usuario'}
          message = {'Está seguro que desea eliminar el usuario seleccionado ?'}
          onClose = {handleCloseUserDeleteDialogFromAction}
        />
      </Dialog>
          
    </>    
  );
}
