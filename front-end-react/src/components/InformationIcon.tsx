import { Tooltip } from '@mui/material';
import InfoOutlinedIcon from '@mui/icons-material/InfoOutlined';

interface Props {
  message: string;
}

export default function InformationIcon({message}:Props) {
  return (
    <Tooltip title={message} arrow placement="top-start">
        <InfoOutlinedIcon fontSize="small" style={{ marginLeft: 8, marginTop: 21, color: 'gray' }} />
    </Tooltip>
  );
}
