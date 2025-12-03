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
import ArrowUpwardIcon from '@mui/icons-material/ArrowUpward'
import ArrowDownwardIcon from '@mui/icons-material/ArrowDownward'
import AddIcon from '@mui/icons-material/Add'
import { useEffect, useState } from 'react';
import SectionAddEditDialog from '../dialogs/SectionAddEditDialog';
import FieldAddEditDialog from '../dialogs/FieldAddEditDialog';
import { Plantilla, Seccion, Campo } from '../types/Plantilla';
import AlertDialog from './AlertDialog';
import { templateService } from '../services/settings/templateService';
import { useSnackbar } from 'notistack';
import ListIcon from '@mui/icons-material/List';
import FieldListValuesDialog from '../dialogs/FieldListValuesDialog';

function SectionRow(
  props: { 
    section: any,
    isFirst: boolean, 
    isLast: boolean,
    editAction: (section: any) => void,
    moveUpAction: () => void,
    moveDownAction: () => void,
    deleteAction: (section: any) => void,
    callbackAction: () => void,
  }
) {
  const { enqueueSnackbar } = useSnackbar(); 

  const { section, isFirst, isLast, editAction, moveUpAction, moveDownAction, deleteAction, callbackAction } = props;
  const [open, setOpen] = React.useState(false);

  // Field
  const [openFieldAddEditDialog, setOpenFieldAddEditDialog] = useState<boolean>(false);
  const [selectedField, setSelectedField] = useState<Campo>();
  const [openFieldDeleteDialog, setOpenFieldDeleteDialog] = useState<boolean>(false);
  const [openAllFieldsDeleteDialog, setOpenAllFieldsDeleteDialog] = useState<boolean>(false);
  const [openFieldListValuesDialog, setOpenFieldListValuesDialog] = useState<boolean>(false);

  const handleNewFieldClick = () => {
    setSelectedField(undefined);
    setOpenFieldAddEditDialog(true);
  }
  
  // Add edit field dialog
  const handleCloseFieldAddEditDialog = () => {
    setOpenFieldAddEditDialog(false);
  }

  const handleCloseFieldAddEditDialogFromAction= async (actionResult: boolean = false) => {
    if(actionResult) {
      callbackAction();
    }
    setOpenFieldAddEditDialog(false);
  }
 
  const handleOpenFieldAddEditDialog = (data: any) => {
    const tempField = section.listaCampos?.find(
      (s: any) => s.idCampo === data.idCampo
    );
    setSelectedField(tempField);
    setOpenFieldAddEditDialog(true);
  };

  // Delete field dialog
  const handleCloseFieldDeleteDialog = () => {   
    setOpenFieldDeleteDialog(false);
  }  

  const handleCloseFieldDeleteDialogFromAction = async (actionResult: boolean) => {
    try {
      if(actionResult && selectedField) {
        await templateService.deleteField(selectedField.idProceso, selectedField.idPlantilla, selectedField.idSeccion, selectedField.idCampo);
        enqueueSnackbar("Campo eliminado.", { variant: "success" });
        callbackAction();      
      }
      setOpenFieldDeleteDialog(false);      
    }
    catch(error: any){

      if(error.response.data === 'CAMPO_VALORES') {
        enqueueSnackbar('No se puede eliminar el campo, posee valores asociados.', { variant: 'error' });
      }
      else {
          enqueueSnackbar('Ocurrió un error al eliminar el campo de la sección.', { variant: 'error' });
      }      
    }

  }

  const handleOpenFieldDeleteDialog = (data: any) => {
    const tempField = section.listaCampos?.find(
      (s: any) => s.idCampo === data.idCampo
    );
    setSelectedField(tempField);
    setOpenFieldDeleteDialog(true);
  }

  // Move field
  const handleMoveField = async (data: any, direction: 'up' | 'down') => {

    if(section && section.listaCampos) {

      const fieldListTemp = [...section.listaCampos];
      const index1 = fieldListTemp.findIndex((f) => f.idCampo === data.idCampo);
      const index2 = direction === 'up' ? index1 - 1 : index1 + 1;      

      [fieldListTemp[index1], fieldListTemp[index2]] = [fieldListTemp[index2], fieldListTemp[index1]];      

      for (let i = 0; i < fieldListTemp.length; i++) {
        fieldListTemp[i].orden = i + 1;
      }

      try {
        await templateService.changeFieldsOrder(fieldListTemp);
        enqueueSnackbar("Orden del campo actualizado.", { variant: "success" });
        callbackAction();            
      }
      catch (error) {
        enqueueSnackbar("Ocurrió un error al actualizar el orden del campo.", { variant: "error" });
      }      
    }

  }
  
  // Field List Values dialog
  const handleOpenFieldValuesDialog = (data: any) => {
    const tempField = section.listaCampos?.find(
      (s: any) => s.idCampo === data.idCampo
    );
    setSelectedField(tempField);
    setOpenFieldListValuesDialog(true);
  };

  const handleCloseFieldListValuesDialog = () => {
    setOpenFieldListValuesDialog(false);
  }

  const handleCloseFieldListValuesDialogFromAction= async (actionResult: boolean = false) => {
    if(actionResult) {
      callbackAction();
    }
    setOpenFieldListValuesDialog(false);
  }
  
  // Delete all fields dialog
  const handleOpenAllFieldsDeleteDialog = () => {
    setOpenAllFieldsDeleteDialog(true);
  }

  const handleCloseAllFieldsDeleteDialog = () => {   
    setOpenAllFieldsDeleteDialog(false);
  }

  const handleCloseAllFieldsDeleteDialogFromAction = async (actionResult: boolean) => {

    try {
      if(actionResult && section) {
        await templateService.deleteAllFieldsFromSection(section.idProceso, section.idPlantilla, section.idSeccion);
        enqueueSnackbar("Campos eliminados correctamente.", { variant: "success" });
        callbackAction();      
      }
          
    }
    catch(error: any){

      if(error.response.data === 'CAMPO_VALORES') {
        enqueueSnackbar('No se pueden eliminar los campos, uno o más campos poseen valores asociados.', { variant: 'error' });
      }
      else {
          enqueueSnackbar('Ocurrió un error al eliminar los campos de la sección.', { variant: 'error' });
      }  
    }

    setOpenAllFieldsDeleteDialog(false);  
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
        <TableCell>
          {section.nombre}
        </TableCell>
        <TableCell>
          {section.descripcion} 
        </TableCell>
        <TableCell>
          <Checkbox checked={section.activa} disabled={true} />
        </TableCell>
        <TableCell>          
          <IconButton onClick={editAction}>
            <Tooltip title="Editar" arrow placement="top-start">
              <EditIcon />
            </Tooltip>
          </IconButton>
          <IconButton onClick={moveUpAction} disabled={isFirst}>
            <Tooltip title="Mover hacia arriba" arrow placement="top-start">
              <ArrowUpwardIcon />
            </Tooltip>
          </IconButton>          
          <IconButton onClick={moveDownAction} disabled={isLast}>
            <Tooltip title="Mover hacia abajo" arrow placement="top-start">
              <ArrowDownwardIcon />
            </Tooltip>
          </IconButton>
          <IconButton onClick={deleteAction}>
            <Tooltip title="Eliminar" arrow placement="top-start">
              <DeleteIcon />
            </Tooltip>
          </IconButton>
        </TableCell>
      </TableRow>

      {/* Contenido collapsable */}
      <TableRow>
        <TableCell style={{ paddingBottom: 0, paddingTop: 0 }} colSpan={6}>
          <Collapse in={open} timeout="auto" unmountOnExit>
            <Box sx={{ margin: "34px" }}>              
              <Alert severity="info" icon={false}>
                <div style={{ marginTop: "8px" }}>
                  Campos de la sección <b>{section.nombre}</b>
                </div>                
              </Alert>

              <Toolbar style={{ paddingLeft: "0px", justifyContent: "space-between" }}>
                <Button
                  size='small'
                  variant="text"
                  color="error"
                  disableElevation
                  startIcon={<DeleteIcon />}
                  onClick={() => { handleOpenAllFieldsDeleteDialog(); }}
                >
                  Eliminar Campos
                </Button>
                <Button 
                  size='small'
                  disableElevation 
                  onClick={() => { handleNewFieldClick(); }}>
                  <AddIcon fontSize="small" /> Nuevo Campo	
                </Button>
              </Toolbar>

              <Table stickyHeader size="small" component={Paper}>
                <TableHead>
                  <TableRow>
                    <TableCell>
                      <b>Nombre</b>
                    </TableCell>
                    <TableCell>
                      <b>Descripción</b>
                    </TableCell>
                    <TableCell>
                      <b>Obligatorio</b>
                    </TableCell>
                    <TableCell>
                      <b>Activo</b>
                    </TableCell>
                    <TableCell style={{ minWidth: 200, textAlign: "center" }}>
                      <b>Acciones</b>
                    </TableCell>
                  </TableRow>
                </TableHead>
                <TableBody>
                  { 
                    section.listaCampos !== null ? (
                      section.listaCampos.map((campo: Campo, index: number) => (
                        <TableRow hover key={campo.idCampo}>
                          <TableCell component="th" scope="row">
                            {campo.nombre}
                          </TableCell>
                          <TableCell>{campo.descripcion}</TableCell>
                          <TableCell>
                            <Checkbox checked={campo.obligatorio} disabled={true} />
                          </TableCell>
                          <TableCell>
                            <Checkbox checked={campo.activo} disabled={true} />
                          </TableCell>
                          <TableCell align="right" sx={{ minWidth: 250 }}>
                            {
                              campo.idTipoCampo !== 6 ? <></> :                              
                                <IconButton onClick={() => {handleOpenFieldValuesDialog(campo)}}>
                                  <Tooltip title="Valores de la lista" arrow placement="top-start">
                                    <ListIcon />
                                  </Tooltip>
                                </IconButton>
                            }                            
                            <IconButton onClick={() => {handleOpenFieldAddEditDialog(campo)}}>
                              <Tooltip title="Editar" arrow placement="top-start">
                                <EditIcon />
                              </Tooltip>
                            </IconButton>                                               
                            <IconButton onClick={() => {handleMoveField(campo, 'up')}} disabled={index === 0}>
                              <Tooltip title="Mover hacia arriba" arrow placement="top-start">
                                <ArrowUpwardIcon />
                              </Tooltip>
                            </IconButton>
                            <IconButton onClick={() => {handleMoveField(campo, 'down')}} disabled={index === section.listaCampos.length - 1}>
                              <Tooltip title="Mover hacia abajo" arrow placement="top-start">
                                <ArrowDownwardIcon />
                              </Tooltip>
                            </IconButton>
                            <IconButton onClick={() => {handleOpenFieldDeleteDialog(campo)}}>
                              <Tooltip title="Eliminar" arrow placement="top-start">
                                <DeleteIcon />
                              </Tooltip>
                            </IconButton>
                          </TableCell>
                        </TableRow>
                      ))
                    ) : <></>
                  }
                </TableBody>
              </Table>
            </Box>
          </Collapse>
        </TableCell>
      </TableRow>

      <Dialog
        open={openFieldAddEditDialog}
        onClose={handleCloseFieldAddEditDialog}
        maxWidth={'sm'}
      >
        <FieldAddEditDialog
          selectedField = {selectedField}
          selectedSection = {section}
          mode={!selectedField ? "add" : "edit"}
          onClose={handleCloseFieldAddEditDialogFromAction}
        />
      </Dialog>

      <Dialog
        open={openFieldDeleteDialog}
        onClose={handleCloseFieldDeleteDialog}
        maxWidth={"sm"}
      >
        <AlertDialog
          color={"error"}
          title={"Eliminar campo"}
          message={
            <>
              ¿Está seguro que desea eliminar el campo: <b>{selectedField?.nombre}</b>? Esta acción no se puede deshacer.
            </>
          }
          onClose={handleCloseFieldDeleteDialogFromAction}
        />
      </Dialog>

      <Dialog
        open={openFieldListValuesDialog}
        onClose={handleCloseFieldListValuesDialog}
        maxWidth={'sm'}
      >
        <FieldListValuesDialog
          selectedField = {selectedField}
          onClose={handleCloseFieldListValuesDialogFromAction}
        />
      </Dialog>

      <Dialog
        open={openAllFieldsDeleteDialog}
        onClose={handleCloseAllFieldsDeleteDialog}
        maxWidth={"sm"}
      >
        <AlertDialog
          color={"error"}
          title={"Eliminar todos campos de las sección"}
          message={
            <>
              ¿Está seguro que desea eliminar todos los campos de la sección <b>{section?.nombre}</b>? Esta acción no se puede deshacer.
            </>
          }
          onClose={handleCloseAllFieldsDeleteDialogFromAction}
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
  selectedTemplate: Plantilla | undefined;
  callbackAction: () => void;
}

export default function SectionsList({
  selectedTemplate,
  callbackAction
}: TableProps) {
  
  const { enqueueSnackbar } = useSnackbar(); 
  const [sectionsList, setSectionsList] = useState<TableColumnType[]>();
  const [openSectionAddEditDialog, setOpenSectionAddEditDialog] = useState<boolean>(false);
  const [selectedSection, setSelectedSection] = useState<Seccion>();
  const [openSectionDeleteDialog, setOpenSectionDeleteDialog] = useState<boolean>(false);
  const [openAllSectionsDeleteDialog, setOpenAllSectionsDeleteDialog] = useState<boolean>(false);
    
  // Section
  const handleCloseSectionAddEditDialog = () => {
    setOpenSectionAddEditDialog(false);
  }

  const handleCloseSectionAddEditDialogFromAction= async (actionResult: boolean = false) => {
    if(actionResult) {
      callbackAction();
    }
    setOpenSectionAddEditDialog(false);
  }

  const handleNewSectionClick = () => {
    setSelectedSection(undefined);
    setOpenSectionAddEditDialog(true);
  }

  const handleOpenSectionAddEditDialog = (data: any) => {
    const tempSection = selectedTemplate?.listaSecciones?.find(
      (s) => s.idSeccion === data.idSeccion
    );
    setSelectedSection(tempSection);
    setOpenSectionAddEditDialog(true);
  };

  // Delete section dialog
  const handleCloseSectionDeleteDialog = () => {   
    setOpenSectionDeleteDialog(false);
  }
  
  // Move section
  const handleMoveSection = async (data: any, direction: 'up' | 'down') => {
    
    if(selectedTemplate && selectedTemplate.listaSecciones) {

      const sectionListTemp = [...selectedTemplate.listaSecciones];
      const index1 = sectionListTemp.findIndex((s) => s.idSeccion === data.idSeccion);
      const index2 = direction === 'up' ? index1 - 1 : index1 + 1;      

      [sectionListTemp[index1], sectionListTemp[index2]] = [sectionListTemp[index2], sectionListTemp[index1]];      

      for (let i = 0; i < sectionListTemp.length; i++) {
        sectionListTemp[i].orden = i + 1;
      }

      try {
        await templateService.changeSectionsOrder(sectionListTemp);
        enqueueSnackbar("Orden de sección actualizado.", { variant: "success" });
        callbackAction();            
      }
      catch (error) {
        enqueueSnackbar("Ocurrió un error al actualizar el orden de la sección.", { variant: "error" });
      }

    }
  }

  const handleCloseSectionDeleteDialogFromAction = async (actionResult: boolean) => {
    try {
      if(actionResult && selectedSection) {
        await templateService.deleteSection(selectedSection.idProceso, selectedSection.idPlantilla, selectedSection.idSeccion);
        enqueueSnackbar("Sección eliminada.", { variant: "success" });
        callbackAction();      
      }
      setOpenSectionDeleteDialog(false);      
    }
    catch(error: any){

      if(error.response.data === 'SECCION_CAMPOS') {
        enqueueSnackbar('No se puede eliminar la sección, posee campos asociados.', { variant: 'error' });
      }
      else {
        enqueueSnackbar('Ocurrió un error al eliminar la sección.', { variant: 'error' });
      }      
    }

  }

  const handleOpenSectionDeleteDialog = (data: any) => {
    const tempSection = selectedTemplate?.listaSecciones?.find(
      (s) => s.idSeccion === data.idSeccion
    );
    setSelectedSection(tempSection);
    setOpenSectionDeleteDialog(true);
  }

  // Delete all sections dialog
  const handleOpenAllSectionsDeleteDialog = () => {
    setOpenAllSectionsDeleteDialog(true);
  }

  const handleCloseAllSectionsDeleteDialog = () => {   
    setOpenAllSectionsDeleteDialog(false);
  }

  const handleCloseAllSectionsDeleteDialogFromAction = async (actionResult: boolean) => {

    try {
      if(actionResult && selectedTemplate) {
        await templateService.deleteAllSectionsFromTemplate(selectedTemplate.idProceso, selectedTemplate.idPlantilla);
        enqueueSnackbar("Secciones eliminadas de la plantilla correctamente.", { variant: "success" });
        callbackAction();      
      }
          
    }
    catch(error: any){

      if(error.response.data === 'SECCION_CAMPOS') {
        enqueueSnackbar('No se pueden eliminar las secciones, una o mas secciones poseen campos asociados.', { variant: 'error' });
      }
      else {
        enqueueSnackbar('Ocurrió un error al eliminar las secciones.', { variant: 'error' });
      }      
    }

    setOpenAllSectionsDeleteDialog(false);  
  }

  useEffect(() => {

    if (selectedTemplate?.listaSecciones && selectedTemplate.listaSecciones.length > 0) {

      const sectionListTemp: any[] = [];

      selectedTemplate.listaSecciones.forEach((section: any) => {
        sectionListTemp.push(section);
      });

      setSectionsList(sectionListTemp);

    } else {
      setSectionsList([]);
    }

  }, [selectedTemplate]);
 

  return (
    <>
      <Toolbar style={{ paddingLeft: "0px", justifyContent: "space-between" }}>
        <Button
          size='small'
          variant="text"
          color="error"
          disableElevation
          startIcon={<DeleteIcon />}
          onClick={() => { handleOpenAllSectionsDeleteDialog(); }}
        >
          Eliminar Secciones
        </Button>
        <Button 
          size='small'
          disableElevation 
          onClick={() => { handleNewSectionClick(); }}>
          <AddIcon fontSize="small" /> Nueva Sección	
        </Button>
      </Toolbar>
      <TableContainer component={Paper}>
        <Table stickyHeader aria-label="collapsible table" size="small">
          <TableHead>
            <TableRow>
              <TableCell
                align={'left'}
                style={{ minWidth: 8 }}
              >
              </TableCell>
              <TableCell
                align={'left'}
                style={{ minWidth: 100 }}
              >
                <b>Nombre</b>
              </TableCell>
              <TableCell
                align={'left'}
                style={{ minWidth: 100 }}
              >
                <b>Descripción</b>
              </TableCell>
              <TableCell
                align={'left'}
                style={{ minWidth: 100 }}
              >
                <b>Activa</b>
              </TableCell>
              <TableCell
                align={'left'}
                style={{ minWidth: 100 }}
              >
                <b>Acciones</b>
              </TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {sectionsList?.map((section, index) => (
              <SectionRow 
                isFirst={index === 0} 
                isLast={index === sectionsList.length - 1}
                section={section} 
                editAction={() => {handleOpenSectionAddEditDialog(section)}}
                moveUpAction={() => {handleMoveSection(section, 'up')}}
                moveDownAction={() => {handleMoveSection(section, 'down')}}
                deleteAction={() => {handleOpenSectionDeleteDialog(section)}}
                callbackAction={callbackAction}
              />
            ))}
          </TableBody>
        </Table>
      </TableContainer>

      <Dialog
        open={openSectionAddEditDialog}
        onClose={handleCloseSectionAddEditDialog}
        maxWidth={'sm'}
      >
        <SectionAddEditDialog
          selectedSection = {selectedSection}
          selectedTemplate = {selectedTemplate}
          mode={!selectedSection ? "add" : "edit"}
          onClose={handleCloseSectionAddEditDialogFromAction}
        />
      </Dialog>

      <Dialog
        open={openSectionDeleteDialog}
        onClose={handleCloseSectionDeleteDialog}
        maxWidth={"sm"}
      >
        <AlertDialog
          color={"error"}
          title={"Eliminar sección"}
          message={
            <>
              ¿Está seguro que desea eliminar la sección: <b>{selectedSection?.nombre}</b>? Esta acción no se puede deshacer.
            </>
          }
          onClose={handleCloseSectionDeleteDialogFromAction}
        />
      </Dialog>

      <Dialog
        open={openAllSectionsDeleteDialog}
        onClose={handleCloseAllSectionsDeleteDialog}
        maxWidth={"sm"}
      >
        <AlertDialog
          color={"error"}
          title={"Eliminar todas las secciones de la plantilla"}
          message={
            <>
              ¿Está seguro que desea eliminar todas las secciones de la plantilla <strong>{selectedTemplate?.nombre}</strong>? Esta acción no se puede deshacer.
            </>
          }
          onClose={handleCloseAllSectionsDeleteDialogFromAction}
        />
      </Dialog>
      
    </>
  );
}