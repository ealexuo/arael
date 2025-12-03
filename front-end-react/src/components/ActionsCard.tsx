import * as React from 'react';
import List from '@mui/material/List';
import ListItem from '@mui/material/ListItem';
import ListItemButton from '@mui/material/ListItemButton';
import ListItemIcon from '@mui/material/ListItemIcon';
import ListItemText from '@mui/material/ListItemText';
import Divider from '@mui/material/Divider';
import { useNavigate } from 'react-router-dom';
import { Paper } from '@mui/material';

interface Action {
  name: string,
  link: string,
  disabled?: boolean
}

interface Props {
  heading: string,
  actions: Action[],
  icon: React.ReactNode
}

export default function ActionsCard({heading, actions, icon}: Props) {

  const navigate = useNavigate();

  return (
    <Paper elevation={4} sx={{ width: 233, maxWidth: 233, height: 233 }}>
      <List>
        <ListItem disablePadding>
          <ListItemIcon sx={{ paddingLeft: 2 }}>
            {icon}
          </ListItemIcon>
          <ListItemText primary={heading} />
        </ListItem>
      </List>
      <Divider />
      <List>
        {actions.map((action, index) => (
          <ListItem disablePadding key={index}>
            <ListItemButton onClick={() => navigate(action.link)} disabled={action.disabled}>
              <ListItemText primary = { action.name } />
            </ListItemButton>
          </ListItem>
        ))}        
      </List>
    </Paper>
  );
}