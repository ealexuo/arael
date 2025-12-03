import React, { Children, PropsWithChildren } from 'react'
import CustomizedBreadcrumbs from './CustomizedBreadcrumbs'
import Typography from '@mui/material/Typography'
import Paper from '@mui/material/Paper';

interface Props {
    title: string
}

export default function Page({title, children}:PropsWithChildren<Props>) {
  return (
    <div>
      <CustomizedBreadcrumbs />
      <Typography
        variant='h5'
        component='h1'
        gutterBottom
        style={{ paddingLeft: 21, textAlign: "left" }}
      >
        {title}
      </Typography>
      <Paper sx={{ width: '100%', overflow: 'hidden', padding: 3}}>
        {children}
      </Paper>
    </div>
  );
}
