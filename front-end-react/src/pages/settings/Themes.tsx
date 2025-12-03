import React from 'react'
import Page from '../../components/Page'
import Switch from '@mui/material/Switch';
import { FormControlLabel, FormGroup, Grid, PaletteMode, Theme, createTheme } from '@mui/material';

type themesProps = {
  selectedTheme: Theme
  setSelectedTheme: (arg: Theme) => void
}

function handleChange(themeMode: PaletteMode): Theme { 
  return createTheme({
    palette: {
      mode: themeMode,
    },
  });
}

export default function Themes({selectedTheme, setSelectedTheme}: themesProps) {
  return (
    <Page title="Themes">
      <Grid container className="center">
        <Grid item justifyContent={'center'}>
          <FormGroup>
            <FormControlLabel control={<Switch checked={selectedTheme.palette.mode === 'light'} />} label="Light Theme" onChange={() => { setSelectedTheme(handleChange('light')) }} />
            <FormControlLabel control={<Switch checked={selectedTheme.palette.mode === 'dark'}/>} label="Dark Theme" onChange={() => { setSelectedTheme(handleChange('dark')) }}/>
          </FormGroup>
        </Grid>
      </Grid>
    </Page>
  );
}
