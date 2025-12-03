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
import { Plantilla, Seccion } from '../types/Plantilla';
import { templateService } from '../services/settings/templateService';

// Dialog parameters Type
type DialogProps = {
    mode: 'add'|'edit',
    selectedSection: Seccion | undefined,
    selectedTemplate: Plantilla | undefined,
    onClose: (refreshSectionsList: boolean) => void
}

// Other global variables

export default function SectionAddEditDialog({mode, selectedSection, selectedTemplate, onClose}: DialogProps) {

    // Local constants or varialbes    
    const [t] = useTranslation();
    const { enqueueSnackbar } = useSnackbar();

    const maxOrder = selectedTemplate && selectedTemplate.listaSecciones && selectedTemplate.listaSecciones.length > 0 ? 
                        Math.max(...selectedTemplate.listaSecciones.map(sec => sec.orden)) : 0
    
    if(!selectedSection){
        selectedSection = {
            idEntidad: selectedTemplate? selectedTemplate.idEntidad: 0,
            idProceso: selectedTemplate? selectedTemplate.idProceso: 0,
            idPlantilla: selectedTemplate? selectedTemplate.idPlantilla: 0,
            idSeccion: 0,
            nombre: '',
            descripcion: '',
            activa: false,
            orden: maxOrder + 1
        }        
    }

    // Form Schema definition
    const formSchema = z.object({
        idEntidad: z.number(),
        idProceso: z.number(),
        idPlantilla: z.number(),
        idSeccion: z.number(),
        nombre: z.string().min(1, t("errorMessages.requieredField")),
        descripcion: z.string(),
        orden: z.number(),
        activa: z.boolean()
    });

    // Form Schema Type
    type UserFormType = z.infer<typeof formSchema>;

    // Form Hook
    const { register, handleSubmit, formState: {errors, isSubmitting} } = useForm<UserFormType>({
        defaultValues: selectedSection,
        resolver: zodResolver(formSchema),
    });

    // For Submit Logic
    const onSubmit: SubmitHandler<UserFormType> = async (formData) => {        
        
        const sectionObject: Seccion = {...formData};
              
        try {
         
          if (mode === "add") {
            
            await templateService.addSection(sectionObject);
            enqueueSnackbar("Sección creada.", { variant: "success" });

          } else {
            
            await templateService.editSection(sectionObject);
            enqueueSnackbar("Seccion actualizada.", { variant: "success" });           

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
                <DialogTitle>{"Información de la Sección"}</DialogTitle>
                <DialogContent>
                    <DialogContentText>Ingrese información de la Sección</DialogContentText>
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
                                                        defaultChecked = {selectedSection?.activa}
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
