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
import { Plantilla } from '../types/Plantilla';
import { Proceso } from '../types/Proceso';
import { templateService } from '../services/settings/templateService';

// Dialog parameters Type
type DialogProps = {
    mode: 'add'|'edit',
    selectedTemplate: Plantilla | undefined,
    selectedWorkflow: Proceso,
    onClose: (refreshWorkflowTemplatesList: boolean) => void
}

// Other global variables

export default function TemplateAddEditDialog({mode, selectedTemplate, selectedWorkflow, onClose}: DialogProps) {

    // Local constants or varialbes    
    const [t] = useTranslation();
    const { enqueueSnackbar } = useSnackbar();

    if(!selectedTemplate){
        selectedTemplate = {
            idEntidad: selectedWorkflow.idEntidad,
            idProceso: selectedWorkflow.idProceso,
            idPlantilla: 0,
            nombre: '',
            descripcion: '',        
            version: 0,
            versionPropuesta: 0,
            activa: false,
            listaSecciones: []
        }
    }

    // Form Schema definition
    const formSchema = z.object({
        idEntidad: z.number(),
        idProceso: z.number(),
        idPlantilla: z.number(),
        nombre: z.string(),
        descripcion: z.string(),        
        version: z.number(),
        versionPropuesta: z.number(),
        activa: z.boolean()
    });

    // Form Schema Type
    type UserFormType = z.infer<typeof formSchema>;

    // Form Hook
    const { register, handleSubmit, formState: {errors, isSubmitting} } = useForm<UserFormType>({
        defaultValues: selectedTemplate,
        resolver: zodResolver(formSchema),
    });

    // For Submit Logic
    const onSubmit: SubmitHandler<UserFormType> = async (formData) => {        
        
        const templateObject: Plantilla = {...formData};
              
        try {
         
          if (mode === "add") {
            
            await templateService.add(templateObject);
            enqueueSnackbar("Plantilla creada.", { variant: "success" });

          } else {
            
            await templateService.edit(templateObject);
            enqueueSnackbar("Plantilla actualizada.", { variant: "success" });           

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
                <DialogTitle>{"Información de la Plantilla"}</DialogTitle>
                <DialogContent>
                    <DialogContentText>Ingrese información de la Plantilla</DialogContentText>
                    <Box sx={{ my: 3 }}>
                        <Typography variant="subtitle1">
                        </Typography>
                        <Paper
                            variant="outlined"
                            sx={{ my: { xs: 2, md: 2 }, p: { xs: 2, md: 3 } }}
                        >
                            <Grid container spacing={3}>
                                <Grid item xs={12}>
                                    <TextField
                                        label="* Nombre"
                                        fullWidth
                                        variant="standard"
                                        {...register("nombre")}
                                        error = { errors.nombre?.message ? true : false }
                                        helperText= { errors.nombre?.message }
                                    />
                                </Grid>
                                <Grid item xs={12}>
                                    <TextField
                                    label="Descripción"
                                    fullWidth
                                    variant="standard"
                                    {...register("descripcion")}
                                    />
                                </Grid> 
                                <Grid item xs={12}>
                                    <Box display="flex" justifyContent="flex-end">
                                        {
                                            mode === 'add' ? 
                                            <></> :
                                            <FormGroup>
                                                <FormControlLabel
                                                    control={
                                                    <Checkbox
                                                        defaultChecked = {selectedTemplate?.activa}
                                                        {...register("activa")}
                                                    />
                                                    }
                                                    label="Activa"
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
