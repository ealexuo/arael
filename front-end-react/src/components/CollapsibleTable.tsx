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
import { Alert, Checkbox, Fab, Toolbar, Tooltip } from '@mui/material';
import DeleteIcon from '@mui/icons-material/Delete'
import EditIcon from '@mui/icons-material/Edit'
import ArrowUpwardIcon from '@mui/icons-material/ArrowUpward'
import ArrowDownwardIcon from '@mui/icons-material/ArrowDownward'
import AddIcon from '@mui/icons-material/Add'
import { useEffect, useState } from 'react';

function Row(props: { row: any, columns: TableColumnType[] | undefined}) {
  const { row } = props;
  const [open, setOpen] = React.useState(false);
  const addACtionToolTip: string = 'Nuevo Campo';
  
  return (
    <React.Fragment>
      <TableRow sx={{}}>
        <TableCell>
          <IconButton
            aria-label="expand row"
            size="small"
            onClick={() => setOpen(!open)}
          >
            {open ? <KeyboardArrowUpIcon /> : <KeyboardArrowDownIcon />}
          </IconButton>
        </TableCell>
        {row.map((item: any, index: number, columns: any) => {
          return typeof item === "object" ? (
            ""
          ) : (
            <TableCell align={columns[index].align} key={index}>
              {typeof item === "boolean" ? (
                <Checkbox checked={item} disabled={true} />
              ) : typeof item === "object" ? (
                ""
              ) : (
                item
              )}
            </TableCell>
          );
        })}
        <TableCell>
          <IconButton disabled={true}>
            <EditIcon />
          </IconButton>
          <IconButton disabled={true}>
            <ArrowUpwardIcon />
          </IconButton>
          <IconButton disabled={true}>
            <ArrowDownwardIcon />
          </IconButton>
          <IconButton disabled={true}>
            <DeleteIcon />
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
                  Campos de la sección <b>{row[1]}</b>
                </div>                
              </Alert>

              <Toolbar style={{ paddingLeft: "0px" }}>
                <Box
                  sx={{
                    display: "flex",
                    alignItems: "flex-end",
                    width: "100%",
                  }}
                ></Box>
                <Tooltip title={addACtionToolTip ? addACtionToolTip : ""}>
                  <Fab size="small" color="primary" aria-label="add">
                    <AddIcon
                      onClick={() => {
                        //onAddActionClick(null);
                      }}
                    />
                  </Fab>
                </Tooltip>
              </Toolbar>

              <Table size="small">
                <TableHead>
                  <TableRow>
                    <TableCell>
                      <b>Nombre</b>
                    </TableCell>
                    <TableCell>
                      <b>Descripción</b>
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
                  {row[4].map((campo: any) => (
                    <TableRow key={campo.idCampo}>
                      <TableCell component="th" scope="row">
                        {campo.nombre}
                      </TableCell>
                      <TableCell>{campo.description}</TableCell>
                      <TableCell>
                        <Checkbox checked={campo.activo} disabled={true} />
                      </TableCell>
                      <TableCell>
                        <IconButton disabled={true}>
                          <EditIcon />
                        </IconButton>
                        <IconButton disabled={true}>
                          <ArrowUpwardIcon />
                        </IconButton>
                        <IconButton disabled={true}>
                          <ArrowDownwardIcon />
                        </IconButton>
                        <IconButton disabled={true}>
                          <DeleteIcon />
                        </IconButton>
                      </TableCell>
                    </TableRow>
                  ))}
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

type TableProps<T> = {
  addActionToolTip?: string,
  onAddActionClick: (item: any) => void,
  data: any[] | undefined;
}

export default function CollapsibleTable({
  addActionToolTip,
  onAddActionClick,
  data,
}: TableProps<any>) {
  
  const [columns, setColumns] = useState<TableColumnType[]>();
  const [rows, setRows] = useState<TableColumnType[]>();

  useEffect(() => {

    setColumns([
      { id: '0', align: 'left', minWidth: 100, label: ''},
      { id: '1', align: 'left', minWidth: 100, label: 'Id Sección'},
      { id: '2', align: 'left', minWidth: 100, label: 'Nombre'},
      { id: '3', align: 'left', minWidth: 100, label: 'Descripción'},
      { id: '4', align: 'left', minWidth: 100, label: 'Estado'},
      { id: '5', align: 'left', minWidth: 100, label: 'Acciones'}
    ]);

    if (data && data.length > 0) {

      const rowsTemp: any[] = [];

      data.forEach((item: any) => {
        rowsTemp.push([
          item.idSeccion,
          item.nombre,
          item.descripcion,
          item.activa,
          item.listaCampos
        ]);
      });

      setRows(rowsTemp);

    } else {
      setRows([]);
    }

  }, [data]);
 

  return (
    <>
      <Toolbar style={{ paddingLeft: "0px" }}>
        <Box
          sx={{
            display: "flex",
            alignItems: "flex-end",
            width: "100%",
          }}
        ></Box>
        <Tooltip title={addActionToolTip ? addActionToolTip : ''}>
          <Fab size="small" color="primary" aria-label="add">
            <AddIcon
              onClick={() => {
                onAddActionClick(null);
              }}
            />
          </Fab>
        </Tooltip>
      </Toolbar>
      <TableContainer component={Paper}>
        <Table aria-label="collapsible table" size="small">
          <TableHead>
            <TableRow>
              {columns?.map((column) => (
                <TableCell
                  key={column.id}
                  align={column.align}
                  style={{ minWidth: column.minWidth }}
                >
                  <b>{column.label}</b>
                </TableCell>
              ))}
            </TableRow>
          </TableHead>
          <TableBody>
            {rows?.map((row, index) => (
              <Row key={index} row={row} columns={columns}/>
            ))}
          </TableBody>
        </Table>
      </TableContainer>
    </>
  );
}