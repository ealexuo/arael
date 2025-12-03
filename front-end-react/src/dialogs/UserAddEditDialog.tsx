import * as React from 'react';
import Button from '@mui/material/Button';
import TextField from '@mui/material/TextField';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';
import DialogContentText from '@mui/material/DialogContentText';
import DialogTitle from '@mui/material/DialogTitle';
import Grid from '@mui/material/Grid/Grid';
import { Autocomplete, Box, Checkbox, FormControlLabel, FormGroup, FormLabel, MenuItem, Paper, Select, Typography } from '@mui/material';
import { useState, useEffect } from 'react';
import { useSnackbar } from 'notistack';

// Form Paso1: Agregar dependencias (ejecutar npm install de ser necesario)
import { SubmitHandler, useForm } from 'react-hook-form';
import { z } from 'zod'
import { zodResolver } from "@hookform/resolvers/zod"
import { userService } from '../services/settings/userService';
import { useTranslation } from 'react-i18next';

type DialogProps = {
  mode: 'add' | 'edit',
  selectedUser: any,
  administrativeUnitsList: any,
  onClose: (refreshUsersList: boolean) => void
}

// TODO: Cargar roles administrativas desde el backend
const roleListInit = [
  { id: 1, name: 'Administrador del Sistema', isSelected: false },
  { id: 2, name: 'Registrador', isSelected: false },
  { id: 3, name: 'Operador', isSelected: false },
  { id: 4, name: 'Consulta', isSelected: false },
  { id: 5, name: 'Consulta Gerencial', isSelected: false }
];

export default function UserAddEditDialog({ mode, selectedUser, administrativeUnitsList, onClose }: DialogProps) {

  console.log("selectedUser: ", selectedUser);
  const [t] = useTranslation();
  const [roleList, setRoleList] = useState<any>(roleListInit);
  const { enqueueSnackbar } = useSnackbar();

  useEffect(() => {
    if (selectedUser && selectedUser.listaRoles) {
      const updatedRoleList = roleList.map((role: { id: number; name: string; isSelected: boolean }) => ({
        ...role,
        isSelected: selectedUser.listaRoles.includes(role.id) // Verificar si el ID está en listaRoles
      }));
      setRoleList(updatedRoleList);
    }
  }, [selectedUser.listaRoles]); // Dependencia para que se ejecute cuando selectedUser .listaRoles cambi

  const handleRoleChange = (roleId: number) => {

    const updatedRoleList = roleList.map((role: any) => {
      if (role.id === roleId) {
        return { ...role, isSelected: !role.isSelected };
      }
      return role;
    });

    setRoleList(updatedRoleList);
  }

  // Form Paso2: Crear un esquema utilizando z de Zod
  const formSchema = z.object({
    idUsuario: z.number(),
    idEntidad: z.number(),
    idIdioma: z.number(),
    genero: z.number(),
    titulo: z.string(),
    noIdentificacionPersonal: z.string().min(1, t("errorMessages.requieredField")), // min(1) se utliza para hacer que el campo sea requerido
    correoElectronico: z.string().email(t("errorMessages.invalidEmail")),
    primerNombre: z.string().min(1, t("errorMessages.requieredField")),
    segundoNombre: z.string(),
    otrosNombres: z.string(),
    primerApellido: z.string().min(1, t("errorMessages.requieredField")),
    segundoApellido: z.string(),
    apellidoCasada: z.string(),
    unidadAdministrativa: z.string().min(1, t("errorMessages.requieredField")),
    cargo: z.string(),
    extension: z.string(),
    telefono: z.string(),
    activo: z.boolean(),
  });

  // Form Paso3: Crar un tipo (type) basado en el esquema creado con Zod anteriormente
  type UserFormType = z.infer<typeof formSchema>;

  // Form Paso4: Utilizar el Form Hook para definir los valores iniciales (defaultValues) y para amarrar el esquema con el resolver(utilizado para las validaciones)
  // 'register' - función que se utiliza en cada input del formulario para amarrar el valor del componente con la propiedad del esquema creado con zod (formSchema)
  // 'handleSubmit' - es una función utilizada por el Form Hook y envolverá la función onSubmit que es la que contiene la lógica al momento de presionar el botón submit
  // 'formState' - proporciona una lista de propiedades para, por ejemplo, acceder a los errores de validación de los inputs y para saber si se está ejecutando el submit
  const { register, handleSubmit, formState: { errors, isSubmitting } } = useForm<UserFormType>({
    defaultValues: selectedUser,
    resolver: zodResolver(formSchema),
  });

  // Form Paso5: función que contiene la lógica a ejecutar al persionar el botón submit
  const onSubmit: SubmitHandler<UserFormType> = async (formData) => {

    let userObject: any = { ...formData }
    let unidadAdministrativaTemp = administrativeUnitsList.find((ua: any) => ua.label === formData.unidadAdministrativa);
    let selectedRoles = roleList.filter((role: any) => role.isSelected);

    userObject.idUnidadAdministrativa = unidadAdministrativaTemp ? unidadAdministrativaTemp.id : 1;
    userObject.listaRoles = selectedRoles ? selectedRoles.map((role: any) => role.id) : [];

    try {

      if (mode === 'add') {
        await userService.add(userObject);
        enqueueSnackbar('Usuario creado.', { variant: 'success' });
      }
      else {
        await userService.edit(userObject.idEntidad, userObject.idUsuario, userObject);
        enqueueSnackbar('Usuario actualizado.', { variant: 'success' });
      }

      onClose(true);

    } catch (error: any) {

      if (error.response?.data) {
        enqueueSnackbar(error.response.data, { variant: 'error' });
      }
      else {
        enqueueSnackbar(error.response.data, { variant: 'error' });
      }

    }

  }

  return (
    <>
      {/* Form Paso6: aqui siempre se llamará a la función handleSubmit enviando como parámetro la función onSubmit */}
      <form onSubmit={handleSubmit(onSubmit)}>
        <DialogTitle>{"User Profile"}</DialogTitle>
        <DialogContent>
          <DialogContentText>Add or Edit User Profile</DialogContentText>

          <Box sx={{ my: 3 }}>
            <Typography variant="subtitle1">
              Información General del Usuario
            </Typography>

            <Paper
              variant="outlined"
              sx={{ my: { xs: 2, md: 2 }, p: { xs: 2, md: 3 } }}
            >
              <Grid container spacing={3}>
                <Grid item xs={12} sm={4}>
                  <FormLabel sx={{ textAlign: "left" }}>* Género</FormLabel>
                  <Select
                    label="* Genero"
                    fullWidth
                    variant="standard"
                    defaultValue={1}
                    // Form Paso7: cada input se debe registrar para amarrar su valor con alguna propiedad del esquema creado con zod
                    {...register("genero")} // por ejemplo "genero" es una propiedad del esquema creado con zod
                  >
                    <MenuItem value={1} selected>Masculino</MenuItem>
                    <MenuItem value={2}>Femenino</MenuItem>
                  </Select>
                </Grid>

                <Grid item xs={12} sm={8}></Grid>

                <Grid item xs={12} sm={4}>
                  <TextField
                    label="Título"
                    fullWidth
                    variant="standard"
                    {...register("titulo")}
                  />
                </Grid>

                <Grid item xs={12} sm={4}>
                  <TextField
                    label="* Identificación Personal"
                    fullWidth
                    variant="standard"
                    {...register("noIdentificacionPersonal")}
                    error={errors.noIdentificacionPersonal?.message ? true : false}
                    helperText={errors.noIdentificacionPersonal?.message}
                  />
                </Grid>

                <Grid item xs={12} sm={4}>
                  <TextField
                    label="* Correo Electrónico"
                    fullWidth
                    variant="standard"
                    {...register("correoElectronico")}
                    error={errors.correoElectronico?.message ? true : false}
                    helperText={errors.correoElectronico?.message}
                  />
                </Grid>

                <Grid item xs={12} sm={4}>
                  <TextField
                    label="* Primer Nombre"
                    fullWidth
                    variant="standard"
                    {...register("primerNombre")}
                    error={errors.primerNombre?.message ? true : false}
                    helperText={errors.primerNombre?.message}
                  />
                </Grid>

                <Grid item xs={12} sm={4}>
                  <TextField
                    label="Segundo Nombre"
                    fullWidth
                    variant="standard"
                    {...register("segundoNombre")}
                  />
                </Grid>

                <Grid item xs={12} sm={4}>
                  <TextField
                    label="Otros Nombres"
                    fullWidth
                    variant="standard"
                    {...register("otrosNombres")}
                  />
                </Grid>

                <Grid item xs={12} sm={4}>
                  <TextField
                    label="* Primer Apellido"
                    fullWidth
                    variant="standard"
                    {...register("primerApellido")}
                    error={errors.primerApellido?.message ? true : false}
                    helperText={errors.primerApellido?.message}
                  />
                </Grid>

                <Grid item xs={12} sm={4}>
                  <TextField
                    label="Segundo Apellido"
                    fullWidth
                    variant="standard"
                    {...register("segundoApellido")}
                  />
                </Grid>

                <Grid item xs={12} sm={4}>
                  <TextField
                    label="Apellido de Casada"
                    fullWidth
                    variant="standard"
                    {...register("apellidoCasada")}
                  />
                </Grid>

                <Grid item xs={12} sm={6}>
                  <Autocomplete
                    disablePortal
                    id="unidadAdministrativa"
                    options={administrativeUnitsList}
                    isOptionEqualToValue={(option: any, value: any) => option.name === value.name}
                    defaultValue={selectedUser.unidadAdministrativa}
                    renderInput={(params) => (
                      <TextField
                        {...params}
                        label="* Unidad Administrativa"
                        variant="standard"
                        {...register("unidadAdministrativa")}
                        error={errors.unidadAdministrativa?.message ? true : false}
                        helperText={errors.unidadAdministrativa?.message}
                      />
                    )}
                  />
                </Grid>

                <Grid item xs={12} sm={6}>
                  <TextField
                    label="Cargo"
                    fullWidth
                    variant="standard"
                    {...register("cargo")}
                  />
                </Grid>

                <Grid item xs={12} sm={6}>
                  <TextField
                    label="Teléfono"
                    fullWidth
                    variant="standard"
                    {...register("telefono")}
                  />
                </Grid>

                <Grid item xs={12} sm={3}>
                  <TextField
                    label="Extensión"
                    fullWidth
                    variant="standard"
                    {...register("extension")}
                  />
                </Grid>

                <Grid item xs={12} sm={3}>
                  {
                    mode === 'add' ?
                      <></> :
                      <FormGroup>
                        <FormControlLabel
                          control={
                            <Checkbox
                              defaultChecked={selectedUser.activo}
                              {...register("activo")}
                            />
                          }
                          label="Activo"
                        />
                      </FormGroup>
                  }
                </Grid>

              </Grid>
            </Paper>

            <Typography variant="subtitle1">
              Roles Asociados al Usuario
            </Typography>

            <Paper
              variant="outlined"
              sx={{ my: { xs: 2, md: 2 }, p: { xs: 2, md: 3 } }}
            >
              <Grid container spacing={3}>
                {
                  roleList.map((role: any) => {
                    return (
                      <Grid key={role.id} item xs={12} sm={4}>
                        <FormGroup>
                          <FormControlLabel
                            control={
                              <Checkbox
                                checked={role.isSelected}
                                onChange={() => handleRoleChange(role.id)}
                              />
                            }
                            label={role.name}
                          />
                        </FormGroup>
                      </Grid>
                    )
                  })
                }
              </Grid>
            </Paper>
          </Box>

        </DialogContent>
        <DialogActions>
          <Button variant="outlined" onClick={() => { onClose(false) }}>
            Cancelar
          </Button>
          {/* Form Paso8: El botón debe ser tipo submit */}
          <Button variant="contained" type="submit" disableElevation disabled={isSubmitting}>
            {isSubmitting ? "Guardando..." : "Guardar"}
          </Button>
        </DialogActions>
      </form>
    </>
  );
}