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
import { Requisito } from '../types/Requisito';
import { Proceso } from '../types/Proceso';

// Toastr Dependencies
import { useSnackbar } from 'notistack';
import { workflowRequirementService } from '../services/settings/workflowRequirementService';

// Dialog parameters Type
type DialogProps = {
    mode: 'add' | 'edit',
    selectedRequirement: Requisito | undefined,
    selectedWorkflow: Proceso | undefined,
    onClose: (refreshSectionsList: boolean) => void
}

// Other global variables

export default function WorkflowRequirementAddEditDialog({ mode, selectedRequirement, selectedWorkflow, onClose }: DialogProps) {

    console.log("selectedRequirement", selectedRequirement);    
    // Local constants or varialbes    
    const [t] = useTranslation();
    const { enqueueSnackbar } = useSnackbar();

    // const maxOrder = selectedWorkflow && selectedTemplate.listaSecciones && selectedTemplate.listaSecciones.length > 0 ? 
    //                     Math.max(...selectedTemplate.listaSecciones.map(sec => sec.orden)) : 0

    const initialRequirement: RequirementFormType = selectedRequirement ?? {
        idEntidad: selectedWorkflow?.idEntidad ?? 0,
        idProceso: selectedWorkflow?.idProceso ?? 0,
        idRequisito: 0,
        requisito: '',
        obligatorio: false,
    };

    // Form Schema definition
    const formSchema = z.object({
        idEntidad: z.number(),
        idProceso: z.number(),
        idRequisito: z.number(),        
        requisito: z.string().min(1, t("errorMessages.requieredField")),        
        obligatorio: z.boolean()
    });

    // Form Schema Type
    type RequirementFormType = z.infer<typeof formSchema>;

    // Form Hook
    const { register, handleSubmit, formState: { errors, isSubmitting } } = useForm<RequirementFormType>({
        defaultValues: initialRequirement,
        resolver: zodResolver(formSchema),
    });

    // For Submit Logic
    const onSubmit: SubmitHandler<RequirementFormType> = async (formData) => {

        const requirementObject: Requisito = {...formData};

        try {

          if (mode === "add") {            
            await workflowRequirementService.add(requirementObject);
            enqueueSnackbar("Requisito creado.", { variant: "success" });

          } else {
            console.log("Esto lleva ",requirementObject);
            await workflowRequirementService.edit(requirementObject);
            enqueueSnackbar("Requisito actualizado.", { variant: "success" });           
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
                <DialogTitle>{"Información del Requisito"}</DialogTitle>
                <DialogContent>
                    <DialogContentText>Ingrese información del Requisito</DialogContentText>
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
                                        label="* Requisito"
                                        fullWidth
                                        variant="standard"
                                        {...register("requisito")}
                                        error={errors.requisito?.message ? true : false}
                                        helperText={errors.requisito?.message}
                                    />
                                </Grid>
                                <Grid item xs={12}>
                                    <Box display="flex" justifyContent="flex-end">
                                        {
                                            <FormGroup>
                                            <FormControlLabel
                                                control={
                                                    <Checkbox
                                                        defaultChecked={selectedRequirement?.obligatorio}
                                                        {...register("obligatorio")}
                                                    />
                                                }
                                                label="Obligatorio"
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
                    <Button variant="outlined" onClick={() => { onClose(false) }}>
                        Cancelar
                    </Button>
                    <Button variant="contained" type="submit" disableElevation disabled={isSubmitting}>
                        {isSubmitting ? "Guardando..." : "Guardar"}
                    </Button>
                </DialogActions>
            </form>
        </>
    )
}
