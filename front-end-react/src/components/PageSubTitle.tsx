import Typography from '@mui/material/Typography'
import React from 'react'

interface Props {
  title: string;
}

export default function PageSubTitle({title}:Props) {
  return (
    <>
      <Typography
        variant='h6'
        component='h2' 
        style={{ textAlign: "left" }}
      >
        {title}
      </Typography>
      <br />
    </>
  );
}
