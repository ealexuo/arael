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
  FormLabel,
  Select,
  MenuItem,
  Autocomplete,
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
import { ColorPicker } from 'primereact/colorpicker';
import { Proceso } from '../types/Proceso';
import { workflowService } from '../services/settings/workflowService';


// Dialog parameters Type
type DialogProps = {
    mode: 'add'|'edit',
    selectedWorkflow: Proceso,
    administrativeUnitsList: any,
    onClose: (refreshWorkflowList: boolean) => void
}

// Other global variables

export default function WorkflowAddEditDialog({mode, selectedWorkflow, administrativeUnitsList, onClose}: DialogProps) {

    // Local constants or varialbes    
    const [t] = useTranslation();
    const { enqueueSnackbar } = useSnackbar();

    // Form Schema definition
    const formSchema = z.object({
        idProceso: z.number(),
        nombre: z.string(),
        descripcion: z.string(),
        idTipoProceso: z.number(),
        unidadAdministrativa: z.string().min(1, t("errorMessages.requieredField")),
        color: z.string(),
        estado: z.boolean(),
    });

    // Form Schema Type
    type UserFormType = z.infer<typeof formSchema>;

    // Form Hook
    const { register, handleSubmit, watch, formState: {errors, isSubmitting} } = useForm<UserFormType>({
        defaultValues: selectedWorkflow,
        resolver: zodResolver(formSchema),
    });

    // Watch the color schema property to be shown in the textfield
    const selectedColor = watch('color', selectedWorkflow.color);

    // For Submit Logic
    const onSubmit: SubmitHandler<UserFormType> = async (formData) => {
        let workflowObject: any = {...formData};
        let unidadAdministrativaTemp = administrativeUnitsList.find((ua: any) => ua.label === formData.unidadAdministrativa);

        workflowObject.idUnidadAdministrativa = unidadAdministrativaTemp ? unidadAdministrativaTemp.id : 1;
        workflowObject.color = workflowObject.color.includes('#') ? workflowObject.color : '#' + workflowObject.color;

        try {
         
          if (mode === "add") {
            
            await workflowService.add(workflowObject);
            enqueueSnackbar("Proceso creado.", { variant: "success" });

          } else {
            
            const response = await workflowService.edit(workflowObject);

            if(response.data === -1) {
                enqueueSnackbar("Para activar un proceso, este debe poseer al menos una sección activa, al menos un campo activo y una fase inicial activa.", { variant: "error" });
            }
            else {
                enqueueSnackbar("Proceso actualizado.", { variant: "success" });
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
                <DialogTitle>{"Información del Proceso"}</DialogTitle>
                <DialogContent>
                    <DialogContentText>Ingrese información general del Proceso</DialogContentText>
                    <Box sx={{ my: 3 }}>
                        <Typography variant="subtitle1">
                        </Typography>
                        <Paper
                            variant="outlined"
                            sx={{ my: { xs: 2, md: 2 }, p: { xs: 2, md: 3 } }}
                        >
                            <Grid container spacing={3}>
                                <Grid item xs={12} sm={6}>
                                    <TextField
                                        label="* Nombre"
                                        fullWidth
                                        variant="standard"
                                        {...register("nombre")}
                                        error = { errors.nombre?.message ? true : false }
                                        helperText= { errors.nombre?.message }
                                    />
                                </Grid>
                                <Grid item xs={12} sm={6}>
                                    <TextField
                                    label="Descripción"
                                    fullWidth
                                    variant="standard"
                                    {...register("descripcion")}
                                    />
                                </Grid>
                                <Grid item xs={12} sm={6}>
                                    <FormLabel sx={{ textAlign: "left" }}>* Tipo de Proceso</FormLabel>
                                    <Select
                                        label="* Tipo Proceso"
                                        fullWidth
                                        variant="standard"
                                        defaultValue={selectedWorkflow.idTipoProceso}                                      
                                        {...register("idTipoProceso")}
                                    > 
                                        <MenuItem value={0} selected>Estándar</MenuItem>
                                        <MenuItem value={1}>Personalizado</MenuItem>
                                    </Select>
                                </Grid>
                                <Grid item xs={12} sm={6}>
                                    <Autocomplete
                                        disablePortal
                                        id="unidadAdministrativa"
                                        options={administrativeUnitsList}
                                        isOptionEqualToValue={(option: any, value: any) => option.name === value.name}
                                        defaultValue={selectedWorkflow.unidadAdministrativa}
                                        renderInput={(params) => (
                                            <TextField
                                                {...params}
                                                label="* Unidad Administrativa"
                                                variant="standard"
                                                {...register("unidadAdministrativa")}
                                                error = { errors.unidadAdministrativa?.message ? true : false }
                                                helperText= { errors.unidadAdministrativa?.message }
                                            />
                                        )}                      
                                    />
                                </Grid>   
                                <Grid item xs={12} sm={6}>
                                    <TextField
                                        required
                                        label="Color"
                                        fullWidth
                                        variant="standard"                                        
                                        value={selectedColor.startsWith('#') ? selectedColor : '#' + selectedColor}
                                        InputProps={{
                                            readOnly: true,                                        
                                        }}                                        
                                    />

                                    <ColorPicker
                                        inputId="cp-hex"
                                        format="hex"
                                        value={selectedWorkflow.color}
                                        //onChange={(e: ColorPickerChangeEvent) => setWorkflowColor(e.value)}
                                        className="mb-3"
                                        inputStyle={{ position: 'absolute', width: '20px' }}
                                        appendTo="self"
                                        {...register("color")}
                                    />
                                </Grid>
                                <Grid item xs={12} sm={6}>
                                <Box display="flex" justifyContent="flex-end">
                                    {
                                        mode === 'add' ? 
                                        <></> :
                                        <FormGroup>
                                            <FormControlLabel
                                                control={
                                                <Checkbox
                                                    defaultChecked = {selectedWorkflow.estado}
                                                    {...register("estado")}
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
