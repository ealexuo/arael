import React from 'react'
import Page from '../../components/Page'
import Grid from '@mui/material/Grid';
import TextField from '@mui/material/TextField';
import { Box, Button, Stack } from '@mui/material';
import PageSubTitle from '../../components/PageSubTitle';
import { useNavigate } from 'react-router-dom';

type UserProfileProps = {
  mode: "add" | "edit" | "read";
};

export default function UserProfile({ mode }: UserProfileProps) {

 const navigate = useNavigate()
  
  return (
    <Page title="User Profile">
      <Box
        component="form"
        sx={{
          "& .MuiTextField-root": { m: 1, width: "25ch" },
        }}
        noValidate
        autoComplete="off"
      >
        <PageSubTitle title="General Information"></PageSubTitle>
        <Grid container paddingBottom={5}>
          <Grid sm={3}>
            <TextField
              required
              id="first-name"
              label="First Name"
              variant="standard"
            />
          </Grid>
          <Grid sm={3}>
            <TextField
              id="middle-name"
              label="Middle Name"
              variant="standard"
            />
          </Grid>
          <Grid sm={3}>
            <TextField
              id="last-name"
              label="Name Name"
              variant="standard"
            />
          </Grid>
          <Grid sm={3}>
            <TextField
              id="email"
              label="Email"
              type='email'
              variant="standard"
            />
          </Grid>
        </Grid>

        <PageSubTitle title="Log In Information"></PageSubTitle>
        <Grid container>
          <Grid sm={3}>
            <TextField
              required
              id="user-name"
              label="User Name"
              variant="standard"
            />
          </Grid>
          <Grid sm={3}>
            {" "}
            <TextField
              id="password"
              label="Password"
              type="password"
              variant="standard"
            />
          </Grid>
          <Grid sm={3}>
            <TextField
              id="confirm-password"
              label="Confirm Password"
              type="password"
              variant="standard"
            />
          </Grid>
        </Grid>
        <Stack
          spacing={2}
          direction="row"
          paddingTop={5}
          justifyContent={"right"}
        >
          <Button variant="outlined" onClick={() => navigate(-1)}>Cancel</Button>
          <Button variant="contained">Save</Button>
        </Stack>
      </Box>
    </Page>
  );
}
