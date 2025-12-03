import { Skeleton, TableCell, TableRow } from '@mui/material';

interface Props {
  columnsNumber: number,
  rowsNumber: number
}

export default function SkeletonTable({columnsNumber, rowsNumber}: Props) {

  return (
    <>
      {
        Array.from({ length: rowsNumber }, (_, index) => (
          <TableRow key={index}>
            {Array.from({ length: columnsNumber }, (_, cellIndex) => (
              <TableCell key={cellIndex}>
                <Skeleton />
              </TableCell>
            ))}
          </TableRow>
        ))
      }      
    </>
  );
}