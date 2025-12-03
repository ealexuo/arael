import React, { useEffect, useState } from 'react'
import TextFieldsIcon from "@mui/icons-material/TextFields"; // Equivalente a fa-text-width
import TitleIcon from "@mui/icons-material/Title"; // Equivalente a fa-text-height
import TagIcon from "@mui/icons-material/Tag"; // Equivalente a fa-slack-hash
import EventIcon from "@mui/icons-material/Event"; // Equivalente a fa-calendar-alt
import CheckBoxIcon from "@mui/icons-material/CheckBox"; // Equivalente a fa-check-square
import ListIcon from "@mui/icons-material/List"; // Equivalente a fa-list
import MonetizationOnIcon from "@mui/icons-material/MonetizationOn"; // Equivalente a fa-coins


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
    Autocomplete,
    ListItemIcon
} from "@mui/material";

// React Form Dependencies
import { SubmitHandler, useForm } from 'react-hook-form';
import { z } from 'zod'
import { zodResolver } from "@hookform/resolvers/zod"
import { useTranslation } from 'react-i18next';

// Toastr Dependencies
import { useSnackbar } from 'notistack';
import { Seccion, Campo } from '../types/Plantilla';
import { templateService } from '../services/settings/templateService';
import InformationIcon from '../components/InformationIcon';

// Dialog parameters Type
type DialogProps = {
    mode: 'add' | 'edit',
    selectedField: Campo | undefined,
    selectedSection: Seccion | undefined,
    onClose: (refreshFieldsList: boolean) => void
}

const fieldSize = [
    { value: 3, label: "Pequeño" },
    { value: 6, label: "Mediano" },
    { value: 9, label: "Largo" },
    { value: 12, label: "Ancho Completo" }
];

export default function FieldAddEditDialog({ mode, selectedField, selectedSection, onClose }: DialogProps) {
    
    const [t] = useTranslation();
    const { enqueueSnackbar } = useSnackbar();

    const [loading, setLoading] = useState<boolean>(false);
    const [fieldTypesList, setFieldTypesList] = useState<any[]>([]);
    const [selectedFieldType, setSelectedFieldType] = React.useState<number>(selectedField ? selectedField.idTipoCampo : 1);
    const [filedsOfTypeListList, setFiledsOfTypeListList] = React.useState<Campo[]>([]);
    const [selectedPartentList, setSelectedParentList] = React.useState<number>(selectedField ? selectedField.idCampoPadre : 0);
    const [selectedFieldSize, setSelectedFieldSize] = React.useState<{ value: number; label: string }>(
        selectedField ? fieldSize.find((item) => item.value === selectedField?.noColumnas) || fieldSize[0] : fieldSize[0]
    );

    const maxOrder = selectedSection && selectedSection.listaCampos && selectedSection.listaCampos.length > 0 ?
        Math.max(...selectedSection.listaCampos.map(field => field.orden)) : 0

    if (!selectedField) {
        selectedField = {
            idEntidad: selectedSection ? selectedSection.idEntidad : 0,
            idProceso: selectedSection ? selectedSection.idProceso : 0,
            idPlantilla: selectedSection ? selectedSection.idPlantilla : 0,
            idSeccion: selectedSection ? selectedSection.idSeccion : 0,
            idCampo: 0,
            nombre: '',
            descripcion: '',
            orden: maxOrder + 1,
            longitud: 0,
            obligatorio: false,
            noColumnas: 0,
            idTipoCampo: 0,
            activo: false,
            idCampoPadre: 0,
            nombreCampoPadre: ''
        }
    }
    else {
        if (!selectedField.nombreCampoPadre) {
            selectedField.nombreCampoPadre = '';
        }        
    }

    // Icons Type
    const iconMap: Record<number, JSX.Element> = {
        1: <TextFieldsIcon />,
        2: <TitleIcon />,
        3: <TagIcon />,
        4: <EventIcon />,
        5: <CheckBoxIcon />,
        6: <ListIcon />,
        7: <MonetizationOnIcon />
    };

    // Form Schema definition
    const formSchema = z.object({
        idEntidad: z.number(),
        idProceso: z.number(),
        idPlantilla: z.number(),
        idSeccion: z.number(),
        idCampo: z.number(),
        nombre: z.string().min(1, t("errorMessages.requieredField")),
        descripcion: z.string(),
        orden: z.number(),
        longitud: z.number(),
        obligatorio: z.boolean(),
        noColumnas: z.number(),
        idTipoCampo: z.number(),
        activo: z.boolean(),
        idCampoPadre: z.number(),
        nombreCampoPadre: z.string(),
    });

    // Form Schema Type
    type UserFormType = z.infer<typeof formSchema>;

    // Form Hook
    const { register, handleSubmit, formState: { errors, isSubmitting } } = useForm<UserFormType>({
        defaultValues: selectedField,
        resolver: zodResolver(formSchema),
    });

    // For Submit Logic
    const onSubmit: SubmitHandler<UserFormType> = async (formData) => {

        const fieldObject: Campo = { ...formData };
        fieldObject.idTipoCampo = selectedFieldType;
        fieldObject.idCampoPadre = selectedPartentList;
        fieldObject.noColumnas = selectedFieldSize.value;

        try {

            if (mode === "add") {

                await templateService.addField(fieldObject);
                enqueueSnackbar("Campo creado.", { variant: "success" });

            } else {

                await templateService.editField(fieldObject);
                enqueueSnackbar("Campo actualizado.", { variant: "success" });

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

    // Fetch functions
    //const fetchTemplates = useCallback(async (workflowId: number) => {
    useEffect(() => {
        const fetchFieldTypes = async () => {
            setLoading(true);
            
            try {
                const response = await templateService.getAllFieldTypes();
                if (response.statusText === "OK") {
                    setLoading(false);
                    setFieldTypesList(response.data);
                } else {
                    enqueueSnackbar("Error al obtener los tipos de campo.", {
                        variant: "error",
                    });
                }
            } catch {
                enqueueSnackbar("Error al obtener tipos de campo.", {
                    variant: "error",
                });
                setLoading(false);
            }
        };

        const fetchFieldOfTypeList = async () => {
            setLoading(true);
            
            try {
                const response = await templateService.getAllFieldsOfTypeList(
                    selectedSection?.idProceso || 0,
                    selectedSection?.idPlantilla || 0,
                    selectedSection?.idSeccion || 0,
                    selectedField?.idCampo || 0
                );
                if (response.statusText === "OK") {
                    setFiledsOfTypeListList(response.data);                    
                } else {
                    enqueueSnackbar("Error al obtener los campos tipo lista.", {
                        variant: "error",
                    });
                }
            } catch {
                enqueueSnackbar("Error al obtener los campos tipo lista.", {
                    variant: "error",
                });
            }
            finally {
                setLoading(false);
            }
        };

        fetchFieldTypes();
        fetchFieldOfTypeList();

    }, [enqueueSnackbar, selectedField, selectedSection]);


    return (
        <>
            <form onSubmit={handleSubmit(onSubmit)}>
                <DialogTitle>{"Información del Campo"}</DialogTitle>
                <DialogContent>
                    <DialogContentText>Ingrese información del Campo</DialogContentText>
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
                                        error={errors.nombre?.message ? true : false}
                                        helperText={errors.nombre?.message}
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
                                    <Autocomplete
                                        options={fieldTypesList}
                                        getOptionLabel={(option) => option.nombre}
                                        value={fieldTypesList.find((item) => item.idTipoCampo === selectedFieldType) || null}
                                        onChange={(event, newValue) => {
                                            setSelectedFieldType(newValue ? newValue.idTipoCampo : 1);
                                        }}
                                        renderOption={(props, option) => (
                                            <li {...props} key={option.idTipoCampo}>
                                                <ListItemIcon>
                                                    {iconMap[Number(option.idTipoCampo)] || <TextFieldsIcon />}
                                                </ListItemIcon>
                                                {option.nombre}
                                            </li>
                                        )}
                                        renderInput={(params) => (
                                            <TextField
                                                {...params}
                                                label="* Tipo de Campo"
                                                variant="standard"
                                                fullWidth
                                            />
                                        )}
                                    />
                                </Grid>

                                {/* Parent list field, only for fieldType = 6 */}
                                {
                                    selectedFieldType === 6 ? (     
                                        <Grid item xs={12}>
                                            <Autocomplete
                                                options={filedsOfTypeListList}
                                                getOptionLabel={(option) => option.nombre}
                                                value={filedsOfTypeListList.find((item: any) => item.idCampo === selectedPartentList) || null}
                                                onChange={(event, newValue) => {
                                                    setSelectedParentList(newValue ? newValue.idCampo : 0);
                                                }}
                                                renderOption={(props, option) => (
                                                    <li {...props} key={option.idCampo}>                                                        
                                                        {option.nombre}
                                                    </li>
                                                )}
                                                renderInput={(params) => (
                                                    <div style={{ display: 'flex' }}>
                                                        <TextField
                                                            {...params}
                                                            label="Lista Predecesora"
                                                            variant="standard"
                                                            fullWidth
                                                        />
                                                        <InformationIcon message="Lista de la cual se desea que dependa la nueva lista que se está creando." />                                                        
                                                    </div>
                                                )}
                                            />
                                        </Grid>
                                    ) : (<></>)
                                }

                                {/* Campo Longitud: SOLO si es Texto o Texto Grande */}
                                {
                                    selectedFieldType === 1 || selectedFieldType === 2 ? (
                                        <Grid item xs={6}>
                                            <TextField
                                                label="Longitud"
                                                fullWidth
                                                variant="standard"
                                                type="number"
                                                inputProps={{ min: 0, max: 250 }}
                                                onInput={(e) => {
                                                    const target = e.target as HTMLInputElement;
                                                    if (Number(target.value) < 0) {
                                                        target.value = "0";
                                                    } else if (Number(target.value) > 250) {
                                                        target.value = "250";
                                                    }
                                                }}
                                                {...register("longitud", {
                                                    setValueAs: (value) => value === "" ? undefined : Math.min(Math.max(Number(value), 0), 250),
                                                })}
                                            />
                                        </Grid>
                                    ) : (<></>)
                                }                                

                                <Grid item xs={6}>
                                    <Autocomplete
                                        disableClearable
                                        options={fieldSize}
                                        getOptionLabel={(option) => option.label}
                                        isOptionEqualToValue={(option: any, value: any) => option.value === value.value}
                                        defaultValue={selectedFieldSize}
                                        onChange={(event, newValue) => {
                                            setSelectedFieldSize(newValue ? newValue : fieldSize[0]);
                                        }}
                                        renderOption={(props, option) => (
                                            <li {...props} key={option.value}>                                                        
                                                {option.label}
                                            </li>
                                        )}
                                        renderInput={(params) => (
                                            <TextField
                                                    {...params}
                                                    label="* Tamaño del Campo"
                                                    variant="standard"
                                                    fullWidth
                                                />   
                                        )}
                                    />
                                </Grid>
                                <Grid item xs={6} >
                                    <Box display="flex" justifyContent="flex-start">
                                        <FormGroup>
                                            <FormControlLabel
                                                control={
                                                    <Checkbox
                                                        defaultChecked={selectedField?.obligatorio}
                                                        {...register("obligatorio")}
                                                    />
                                                }
                                                label="Obligartorio"
                                            />
                                        </FormGroup>
                                    </Box>
                                </Grid>
                                <Grid item xs={6}>
                                    <Box display="flex" justifyContent="flex-start">
                                        {
                                            mode === 'add' ?
                                                <></> :
                                                <FormGroup>
                                                    <FormControlLabel
                                                        control={
                                                            <Checkbox
                                                                defaultChecked={selectedField?.activo}
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
                    <Button variant="outlined" onClick={() => { onClose(false) }}>
                        Cancelar
                    </Button>
                    <Button variant="contained" type="submit" disableElevation disabled={isSubmitting}>
                        {isSubmitting ? "Guadrando..." : "Guardar"}
                    </Button>
                </DialogActions>
            </form>
        </>
    )
}
