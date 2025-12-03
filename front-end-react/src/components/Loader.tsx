import Box from '@mui/material/Box'
import CircularProgress from '@mui/material/CircularProgress'
import React from 'react'

export default function Loader() {
  return (
    <Box sx={{ display: 'block' }}>
        <CircularProgress />
    </Box>
  )
}
