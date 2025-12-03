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
import { Alert, Checkbox, Dialog, Toolbar, Tooltip } from '@mui/material';
import { useEffect, useState } from 'react';
import { Plantilla, Seccion, Campo } from '../types/Plantilla';

function SectionRow(
  props: { 
    section: any,
    isFirst: boolean, 
    isLast: boolean
  }
) {

  const { section } = props;
  const [open, setOpen] = React.useState(false);

  // Field
  const [openFieldAddEditDialog, setOpenFieldAddEditDialog] = useState<boolean>(false);
  const [selectedField, setSelectedField] = useState<Campo>();
  const [openFieldDeleteDialog, setOpenFieldDeleteDialog] = useState<boolean>(false);
  const [openFieldListValuesDialog, setOpenFieldListValuesDialog] = useState<boolean>(false);

  const handleNewFieldClick = () => {
    setSelectedField(undefined);
    setOpenFieldAddEditDialog(true);
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
                      <b>Estado</b>
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
}

export default function SectionsListViewer({
  selectedTemplate
}: TableProps) {
  
; 
  const [sectionsList, setSectionsList] = useState<TableColumnType[]>();

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
                <b>Estado</b>
              </TableCell>             
            </TableRow>
          </TableHead>
          <TableBody>
            {sectionsList?.map((section, index) => (
              <SectionRow 
                isFirst={index === 0} 
                isLast={index === sectionsList.length - 1}
                section={section}
              />
            ))}
          </TableBody>
        </Table>
      </TableContainer>      
    </>
  );
}