import React, { useCallback, useEffect, useState } from 'react'

import Box from '@mui/material/Box';
import Collapse from '@mui/material/Collapse';
import IconButton from '@mui/material/IconButton';
import Table from '@mui/material/Table';
import TableBody from '@mui/material/TableBody';
import TableCell from '@mui/material/TableCell';
import TableContainer from '@mui/material/TableContainer';
import TableHead from '@mui/material/TableHead';
import TableRow from '@mui/material/TableRow';
import Typography from '@mui/material/Typography';
import Paper from '@mui/material/Paper';
import KeyboardArrowDownIcon from '@mui/icons-material/KeyboardArrowDown';
import KeyboardArrowUpIcon from '@mui/icons-material/KeyboardArrowUp';
import Page from '../../components/Page';
import { Alert, Button, Chip, Dialog, TablePagination, TextField, Toolbar, Tooltip } from '@mui/material';
import { DeviceHub, Edit } from '@mui/icons-material';
import AddIcon from '@mui/icons-material/Add'
import FileCreateDialog from '../../dialogs/FileCreateDialog';
import FileCreateDialogTemp from '../../dialogs/FileCreateDialogTemp';
import EditIcon from '@mui/icons-material/Edit';
import DeleteIcon from '@mui/icons-material/Delete'
import AlertDialog from '../../components/AlertDialog';
import SearchIcon from '@mui/icons-material/Search';
import EditNoteIcon from '@mui/icons-material/EditNote';
import ContentPasteSearchIcon from '@mui/icons-material/ContentPasteSearch';
import FileSIADSearchDialog from '../../dialogs/FileSIADSearchDialog';
import FileCreateNoteDialog from '../../dialogs/FileCreateNoteDialog';

function createData(
  code: string,
  product: string,
  process: string,
  siad: number,
  key: string,
  client: string,
  receivedDate: string,
  notesCount: number,
  lastUpdateDate: string,
) {
  return {
    code,
    product,
    process,
    siad,
    key,
    client,
    receivedDate,
    notesCount,
    lastUpdateDate,
    notes: [
      {
        date: '05/12/2025',
        datelimit: '05/03/2026',
        page: '9,25',
        description: 'Descripción de rechazo de la nota 1',
      },
      {
        date: '05/12/2025',
        datelimit: '05/03/2026',
        page: '17,18',
        description: 'Descripción de rechazo de la nota 2',
      },
    ],
  };
}

function DebounceTextField({ handleDebounce, debounceTimeout }: any) {

  const timerRef = React.useRef<ReturnType<typeof setTimeout>>();

  const handleChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    clearTimeout(timerRef.current);
    timerRef.current = setTimeout(() => {
      handleDebounce(event);
    }, debounceTimeout);
  };

  return (
    <Box sx={{ display: "flex", alignItems: "flex-end", width: '100%' }}>
      <TextField
        id="input-with-sx"
        label="Buscar"
        variant="standard"
        sx={{ width: 300, marginLeft: '15px' }}
        onChange={handleChange}
      />
      <SearchIcon sx={{ color: "action.active", mr: 1, my: 0.5 }} />
    </Box>
  );
}

function Row(props: { row: ReturnType<typeof createData> }) {
  const { row } = props;
  const [open, setOpen] = React.useState(false);

   const [openFileCreateDialog, setOpenFileCreateDialog] = useState<boolean>(false);
   const [openAllSectionsDeleteDialog, setOpenAllSectionsDeleteDialog] = useState<boolean>(false);
   const [openFileSIADSearchDialog, setOpenFileSIADSearchDialog] = useState<boolean>(false);
   const [openFileCreateNoteDialog, setOpenFileCreateNoteDialog] = useState<boolean>(false);

   const handleCloseFileCreateNoteDialog = () => {
    setOpenFileCreateNoteDialog(false);
  }

  const handleOpenFileCreateNoteDialog = () => {
    setOpenFileCreateNoteDialog(true);
  }

   const handleCloseFileSIADSearchDialog = () => {
    setOpenFileSIADSearchDialog(false);
  }
  
  const handleOpenFileSIADSearchDialog = async () => {
    setOpenFileSIADSearchDialog(true);
  }

  const handleOpenFileCreate = async () => {
    setOpenFileCreateDialog(true);
  }
  
  const handleCloseFileCreateDialog = () => {
    setOpenFileCreateDialog(false);
  }
  
  const handleDeleteFileOpenAllSectionsDeleteDialog = () => {
    setOpenAllSectionsDeleteDialog(true);
  }

  const handleCloseAllSectionsDeleteDialog = () => {   
    setOpenAllSectionsDeleteDialog(false);
  }

  return (
    <React.Fragment>      
        <TableRow hover sx={{ '& > *': { borderBottom: 'unset' } }}>
          <TableCell>
            <IconButton
              aria-label="expand row"
              size="small"
              onClick={() => setOpen(!open)}
            >
              {open ? <KeyboardArrowUpIcon /> : <KeyboardArrowDownIcon />}
            </IconButton>
          </TableCell>

          <TableCell>{row.code}</TableCell>

          <TableCell component="th" scope="row">
            {row.product}
          </TableCell>

          <TableCell align="center">{
              row.process === 'LSN' ? 
                (<Chip label="LSN" color="primary" variant="outlined"/>) : 
                (<Chip label="MOH" color="secondary" variant="outlined"/>)
            }
          </TableCell>
          <TableCell>{row.siad}</TableCell>
          <TableCell>{row.key}</TableCell>
          {/* <TableCell>{row.client}</TableCell> */}
          <TableCell>{row.receivedDate}</TableCell>
          <TableCell>{
              row.notesCount === 1 ?
                (<Alert severity="success">1</Alert>) : 
                  row.notesCount === 2 ? (<Alert severity="warning">2</Alert>) :
                    (<Alert severity="error">3</Alert>)
            }
          </TableCell>
          <TableCell>{row.lastUpdateDate}</TableCell>
          <TableCell>
            <IconButton onClick={() => handleOpenFileCreateNoteDialog()}>
              <Tooltip title="Agregar Nota" arrow placement="top-start">
                <EditNoteIcon />
              </Tooltip>
            </IconButton>            
            <IconButton onClick={() => handleOpenFileCreate()}>
                <Tooltip title="Editar Expediente" arrow placement="top-start">
                  <EditIcon />
                </Tooltip>
            </IconButton>
            <IconButton onClick={() => handleDeleteFileOpenAllSectionsDeleteDialog()}>
              <Tooltip title="Eliminar Expediente" arrow placement="top-start">
                <DeleteIcon />
              </Tooltip>
            </IconButton>
            <IconButton onClick={() => handleOpenFileSIADSearchDialog()}>
              <Tooltip title="Consulta SIAD" arrow placement="top-start">
                <ContentPasteSearchIcon />
              </Tooltip>
            </IconButton>            
          </TableCell>
        </TableRow>
        <TableRow>
          <Paper>
          </Paper>
          <TableCell style={{ paddingBottom: 0, paddingTop: 0 }} colSpan={6}>
            <Collapse in={open} timeout="auto" unmountOnExit>
              <Box sx={{ margin: "34px" }}>
                <Typography variant="h6" gutterBottom component="div">
                  Notas
                </Typography>
                <Table size="small" aria-label="purchases">
                  <TableHead>
                    <TableRow>
                      <TableCell><strong>Fecha Recepción</strong></TableCell>
                      <TableCell><strong>Fecha Límite</strong></TableCell>
                      <TableCell align="right"><strong>Páginas</strong></TableCell>
                      <TableCell><strong>Descripción</strong></TableCell>
                    </TableRow>
                  </TableHead>
                  <TableBody>
                    {row.notes.map((note) => (
                      <TableRow key={note.date}>
                        <TableCell component="th" scope="row">
                          {note.date}
                        </TableCell>
                         <TableCell component="th" scope="row">
                          {note.datelimit}
                        </TableCell>
                        <TableCell align="right">{note.page}</TableCell>
                        <TableCell>{note.description}</TableCell>
                        
                      </TableRow>
                    ))}
                  </TableBody>
                </Table>
              </Box>
            </Collapse>
          </TableCell>
        </TableRow>

        <Dialog
          open={openFileCreateDialog}
          onClose={handleCloseFileCreateDialog}
          maxWidth={"md"}
        >
          <FileCreateDialogTemp 
            onClose={handleCloseFileCreateDialog}
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
                ¿Está seguro que desea eliminar el expediente <strong>PROV001-10-2025</strong>? Esta acción no se puede deshacer.
              </>
            }
            onClose={handleCloseAllSectionsDeleteDialog}
          />
        </Dialog>
        
        <Dialog
          open={openFileCreateDialog}
          onClose={handleCloseFileCreateDialog}
          maxWidth={"md"}
        >
          <FileCreateDialogTemp 
            onClose={handleCloseFileCreateDialog}
          />
        </Dialog>

        <Dialog
          open={openFileSIADSearchDialog}
          onClose={handleCloseFileSIADSearchDialog}
          maxWidth={"lg"}        
        >
          <FileSIADSearchDialog 
            onClose={handleCloseFileSIADSearchDialog}
          />
        </Dialog>

        <Dialog
          open={openFileCreateNoteDialog}
          onClose={handleCloseFileCreateNoteDialog}
          maxWidth={"lg"}        
        >
          <FileCreateNoteDialog 
            onClose={handleCloseFileCreateNoteDialog}
          />
        </Dialog>

    </React.Fragment>
  );
}
const rows = [
  createData('PROV004-01-2025', 'IBUPROFENO NORMON 600 MG COMPRIMIDOS', 'LSN', 256487, 'JB8SH76', 'PROV004', '12/03/2025', 2, '05/01/2025'),
  createData('PROV004-01-2025', 'IBUPROFENO NORMON 600 MG COMPRIMIDOS', 'MOH', 256487, 'JB8SH76', 'PROV004', '12/03/2025', 3, '22/04/2025'),

  createData('PROV002-02-2025', 'PARACETAMOL CINFA 1 G COMPRIMIDOS EFERVESCENTES', 'LSN', 698523, 'JUE938F', 'PROV002', '25/07/2025', 1, '14/06/2025'),
  createData('PROV002-02-2025', 'PARACETAMOL CINFA 1 G COMPRIMIDOS EFERVESCENTES', 'MOH', 698523, 'JUE938F', 'PROV002', '25/07/2025', 2, '03/03/2025'),
  
  createData('PROV006-03-2025', 'OMEPRAZOL SANDOZ 20 MG CÁPSULAS GASTRORRESISTENTES', 'LSN', 249875, 'J837HD0', 'PROV006', '03/02/2025', 3, '29/01/2025'),
  createData('PROV006-04-2025', 'OMEPRAZOL SANDOZ 20 MG CÁPSULAS GASTRORRESISTENTES', 'MOH', 249875, 'J837HD0', 'PROV006', '03/02/2025', 1, '12/04/2025'),

  createData('PROV003-04-2025', 'AMOXICILINA NORMON 500 MG CÁPSULAS', 'LSN', 369852, 'K926F83', 'PROV003', '16/10/2025', 2, '09/08/2025'),
  createData('PROV003-04-2025', 'AMOXICILINA NORMON 500 MG CÁPSULAS', 'MOH', 369852, 'K926F83', 'PROV003', '16/10/2025', 1, '22/10/2025'),  

  createData('PROV001-05-2025', 'ATORVASTATINA CINFA 20 MG COMPRIMIDOS RECUBIERTOS', 'LSN', 159874, '8HD532D', 'PROV001', '08/05/2025', 1, '19/02/2025'),
  createData('PROV001-05-2025', 'ATORVASTATINA CINFA 20 MG COMPRIMIDOS RECUBIERTOS', 'MOH', 159874, '8HD532D', 'PROV001', '08/05/2025', 3, '27/09/2025'),

  // createData('PROV005-06-2025', 'METFORMINA TEVA 850 MG COMPRIMIDOS', 'LSN', 369145, 'DS8GEID', 'PROV005', '29/09/2025', 2, '10/03/2025'),
  // createData('PROV005-06-2025', 'METFORMINA TEVA 850 MG COMPRIMIDOS', 'MOH', 369145, 'DS8GEID', 'PROV005', '29/09/2025', 1, '01/07/2025'),

  // createData('PROV007-07-2025', 'LORATADINA KERN PHARMA 10 MG COMPRIMIDOS', 'LSN', 981364, '237DUFP', 'PROV007', '04/04/2025', 3, '22/01/2025'),
  // createData('PROV007-07-2025', 'LORATADINA KERN PHARMA 10 MG COMPRIMIDOS', 'MOH', 981364, '237DUFP', 'PROV007', '04/04/2025', 2, '18/05/2025'),

  // createData('PROV002-08-2025', 'QUETIAPINA CINFA 100 MG COMPRIMIDOS RECUBIERTOS', 'LSN', 165234, '7H37DT3', 'PROV002', '11/08/2025', 1, '05/11/2024'),
  // createData('PROV002-08-2025', 'QUETIAPINA CINFA 100 MG COMPRIMIDOS RECUBIERTOS', 'MOH', 165234, '7H37DT3', 'PROV002', '11/08/2025', 2, '13/02/2025'),

  // createData('PROV006-09-2025', 'PANTOPRAZOL STADA 40 MG COMPRIMIDOS GASTRORRESISTENTES', 'LSN', 361258, '876SE78', 'PROV006', '21/12/2025', 3, '09/06/2025'),
  // createData('PROV006-09-2025', 'PANTOPRAZOL STADA 40 MG COMPRIMIDOS GASTRORRESISTENTES', 'MOH', 361258, '876SE78', 'PROV006', '21/12/2025', 1, '17/10/2025'),

  // createData('PROV003-10-2025', 'SERTRALINA TEVA 50 MG COMPRIMIDOS', 'LSN', 394125, 'MVS089Y', 'PROV003', '19/06/2025', 2, '24/03/2025'),
  // createData('PROV003-10-2025', 'SERTRALINA TEVA 50 MG COMPRIMIDOS', 'MOH', 394125, 'MVS089Y', 'PROV003', '19/06/2025', 3, '02/09/2025')

];

export default function CollapsibleTable() {

  const [openFileCreateDialog, setOpenFileCreateDialog] = useState<boolean>(false);
  const [page, setPage] = useState(0);
  const [rowsPerPage, setRowsPerPage] = useState(5);

  const handleOpenFileCreate = async () => {
    setOpenFileCreateDialog(true);
  }
  
  const handleCloseFileCreateDialog = () => {
    setOpenFileCreateDialog(false);
  }

  const handleChangePage = (event: unknown, newPage: number) => {
    setPage(newPage);
  };

  const handleChangeRowsPerPage = (event: React.ChangeEvent<HTMLInputElement>) => {
    setRowsPerPage(parseInt(event.target.value, 10));
    setPage(0);
  };

  const onSearchTextChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    //setSearchText(event.target.value);
  };
  
  return (
    <>
    
  <Page title="Listado de Expedientes">
      <Toolbar style={{ paddingLeft: "0px", marginBottom: "15px"}} component={Paper}>

        <DebounceTextField          
          handleDebounce={onSearchTextChange}
          debounceTimeout={1000}
        >
        </DebounceTextField>
        <Button 
          size='small'
          disableElevation 
          onClick={() => { handleOpenFileCreate(); }}>
          <AddIcon fontSize="small" /> Crear Expediente	
        </Button>
        
      </Toolbar>
      <TableContainer component={Paper}>
        <Table stickyHeader size='small' component={Paper} aria-label="collapsible table">
          <TableHead>
            <TableRow>
              <TableCell />
              <TableCell><strong>CÓDIGO</strong></TableCell>
              <TableCell><strong>PRODUCTO</strong></TableCell>
              <TableCell align='center'><strong>PROCESO</strong></TableCell>
              <TableCell><strong>SIAD</strong></TableCell>
              <TableCell><strong>LLAVE</strong></TableCell>
              {/* <TableCell><strong>PROVEEDOR</strong></TableCell> */}
              <TableCell><strong>FECHA DE INGRESO</strong></TableCell>
              <TableCell><strong>NOTAS</strong></TableCell>
              <TableCell><strong>FECHA LÍMITE</strong></TableCell>
              <TableCell><strong>ACCIONES</strong></TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {rows.map((row, index) => (
              <Row key={index} row={row} />
            ))}
          </TableBody>
        </Table>
      </TableContainer>
      <TablePagination
          rowsPerPageOptions={[5, 10, 25]}
          component="div"
          count={rows.length}
          rowsPerPage={10}
          page={page}
          onPageChange={handleChangePage}
          onRowsPerPageChange={handleChangeRowsPerPage}
          labelRowsPerPage="Expedientes por página"
        />
  </Page>

    <Dialog
      open={openFileCreateDialog}
      onClose={handleCloseFileCreateDialog}
      maxWidth={"md"}
    >
      <FileCreateDialogTemp 
        onClose={handleCloseFileCreateDialog}
      />
    </Dialog>

  </>
  );
}