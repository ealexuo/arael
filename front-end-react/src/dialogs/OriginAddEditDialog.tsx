import React from 'react'

// General Dependencies
import {
  DialogContent,
  DialogTitle,
  DialogContentText,
  Box,
  DialogActions,
  Button,
  Typography,
  Paper,
  Grid,
  TextField,  
  FormGroup,
  FormControlLabel,
  Checkbox,
} from "@mui/material";

// React Form Dependencies
import { SubmitHandler, useForm } from 'react-hook-form';
import { z } from 'zod'
import { zodResolver } from "@hookform/resolvers/zod"
import { useTranslation } from 'react-i18next';

// Toastr Dependencies
import { useSnackbar } from 'notistack';
import { Origen } from '../types/Origen';
import { originService } from '../services/settings/originService';


// Dialog parameters Type
type DialogProps = {
    mode: 'add'|'edit',
    selectedOrigin: Origen,    
    onClose: (refreshOriginList: boolean) => void
}

// Other global variables

export default function OriginAddEditDialog({mode, selectedOrigin, onClose}: DialogProps) {
        
    // Local constants or varialbes    
    const [t] = useTranslation();
    const { enqueueSnackbar } = useSnackbar();

    // Form Schema definition
    const formSchema = z.object({
        idOrigen: z.number(),
        nombre: z.string(),
        activo: z.boolean()
    });

    // Form Schema Type
    type OriginFormType = z.infer<typeof formSchema>;

    // Form Hook
    const { register, handleSubmit, watch, formState: {errors, isSubmitting} } = useForm<OriginFormType>({
        defaultValues: selectedOrigin,
        resolver: zodResolver(formSchema),
    });    

    // For Submit Logic
    const onSubmit: SubmitHandler<OriginFormType> = async (formData) => {
        let originObject: any = {...formData};                

        try {
         
          if (mode === "add") {
            
            await originService.add(originObject);
            enqueueSnackbar("Origen creado.", { variant: "success" });

          } else {
            
            const response = await originService.edit(originObject);

            if(response.data !== -1) {                
                enqueueSnackbar("Origen actualizado.", { variant: "success" });
            }           

          }

          onClose(true);

        } catch (error: any) {

          if (error.response?.data) {
            enqueueSnackbar(error.response.data, { variant: "error" });
          } else {
            enqueueSnackbar(error.response.data, { variant: "error" });
          }

        }
    }

    return (
        <>
            <form onSubmit={handleSubmit(onSubmit)}> 
                <DialogTitle>{"Información del Origen"}</DialogTitle>
                <DialogContent>
                    <DialogContentText>Ingrese información general del Origen</DialogContentText>
                    <Box sx={{ my: 3 }}>
                        <Typography variant="subtitle1">
                        </Typography>
                        <Paper
                            variant="outlined"
                            sx={{ my: { xs: 2, md: 2 }, p: { xs: 2, md: 3 } }}
                        >
                            <Grid container spacing={3}>
                                <Grid item xs={12} sm={12}>
                                    <TextField
                                        label="* Origen"
                                        fullWidth
                                        variant="standard"
                                        {...register("nombre")}
                                        error = { errors.nombre?.message ? true : false }
                                        helperText= { errors.nombre?.message }
                                    />
                                </Grid>
                                <Grid item xs={12} sm={12}>
                                <Box display="flex" justifyContent="flex-end">
                                    {
                                        mode === 'add' ? 
                                        <></> :
                                        <FormGroup>
                                            <FormControlLabel
                                                control={
                                                <Checkbox
                                                    defaultChecked = {selectedOrigin.activo}
                                                    {...register("activo")}
                                                />
                                                }
                                                label="Activo"
                                            />
                                        </FormGroup>
                                    }
                                </Box>
                                                        
                                </Grid>
                            </Grid>
                        </Paper>
                    </Box>
                </DialogContent>
                <DialogActions>
                    <Button variant="outlined" onClick={() => {onClose(false)}}>
                        Cancelar
                    </Button>
                    <Button variant="contained" type="submit" disableElevation disabled = {isSubmitting}>
                        {isSubmitting ? "Guardando..." : "Guardar"}
                    </Button>
                </DialogActions>
            </form>
        </>
    )
}
