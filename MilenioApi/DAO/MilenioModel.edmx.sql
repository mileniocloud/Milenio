
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 01/05/2019 11:46:39
-- Generated from EDMX file: C:\Proyectos\MilenioCloud\Milenio-master\MilenioApi\DAO\MilenioModel.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [MilenioCloud];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_CAula]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Curso] DROP CONSTRAINT [FK_CAula];
GO
IF OBJECT_ID(N'[dbo].[FK_CSubcategoria]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Subcategoria] DROP CONSTRAINT [FK_CSubcategoria];
GO
IF OBJECT_ID(N'[dbo].[FK_EEntidad_Padre]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Entidad] DROP CONSTRAINT [FK_EEntidad_Padre];
GO
IF OBJECT_ID(N'[dbo].[FK_EUbicacion]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Entidad] DROP CONSTRAINT [FK_EUbicacion];
GO
IF OBJECT_ID(N'[dbo].[FK_IArticulo]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Imagen] DROP CONSTRAINT [FK_IArticulo];
GO
IF OBJECT_ID(N'[dbo].[FK_IPersona]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Imagen] DROP CONSTRAINT [FK_IPersona];
GO
IF OBJECT_ID(N'[dbo].[FK_LEntidad]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Licencia] DROP CONSTRAINT [FK_LEntidad];
GO
IF OBJECT_ID(N'[dbo].[FK_MDepartamento]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Municipio] DROP CONSTRAINT [FK_MDepartamento];
GO
IF OBJECT_ID(N'[dbo].[FK_Persona_Rol_Entidad]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Entidad_Persona_Rol] DROP CONSTRAINT [FK_Persona_Rol_Entidad];
GO
IF OBJECT_ID(N'[dbo].[FK_PMunicipio]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Poblado] DROP CONSTRAINT [FK_PMunicipio];
GO
IF OBJECT_ID(N'[dbo].[FK_PTipoIdentificacion]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Persona] DROP CONSTRAINT [FK_PTipoIdentificacion];
GO
IF OBJECT_ID(N'[dbo].[FK_PUbicacion]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Persona] DROP CONSTRAINT [FK_PUbicacion];
GO
IF OBJECT_ID(N'[dbo].[FK_Telefono_Persona]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Telefono] DROP CONSTRAINT [FK_Telefono_Persona];
GO
IF OBJECT_ID(N'[dbo].[FK_TEntidad]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Telefono] DROP CONSTRAINT [FK_TEntidad];
GO
IF OBJECT_ID(N'[dbo].[FK_UPoblado]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Ubicacion] DROP CONSTRAINT [FK_UPoblado];
GO
IF OBJECT_ID(N'[dbo].[FK_Usuario_Rol_Persona1]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Entidad_Persona_Rol] DROP CONSTRAINT [FK_Usuario_Rol_Persona1];
GO
IF OBJECT_ID(N'[dbo].[FK_Usuario_Rol_Rol]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Entidad_Persona_Rol] DROP CONSTRAINT [FK_Usuario_Rol_Rol];
GO
IF OBJECT_ID(N'[dbo].[FK_USubcategoria]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Articulo] DROP CONSTRAINT [FK_USubcategoria];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Articulo]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Articulo];
GO
IF OBJECT_ID(N'[dbo].[Aula]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Aula];
GO
IF OBJECT_ID(N'[dbo].[Categoria]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Categoria];
GO
IF OBJECT_ID(N'[dbo].[Curso]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Curso];
GO
IF OBJECT_ID(N'[dbo].[Departamento]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Departamento];
GO
IF OBJECT_ID(N'[dbo].[Entidad]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Entidad];
GO
IF OBJECT_ID(N'[dbo].[Entidad_Persona_Rol]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Entidad_Persona_Rol];
GO
IF OBJECT_ID(N'[dbo].[GenericError]', 'U') IS NOT NULL
    DROP TABLE [dbo].[GenericError];
GO
IF OBJECT_ID(N'[dbo].[Imagen]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Imagen];
GO
IF OBJECT_ID(N'[dbo].[Licencia]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Licencia];
GO
IF OBJECT_ID(N'[dbo].[Municipio]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Municipio];
GO
IF OBJECT_ID(N'[dbo].[Persona]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Persona];
GO
IF OBJECT_ID(N'[dbo].[Poblado]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Poblado];
GO
IF OBJECT_ID(N'[dbo].[Rol]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Rol];
GO
IF OBJECT_ID(N'[dbo].[Subcategoria]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Subcategoria];
GO
IF OBJECT_ID(N'[dbo].[Telefono]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Telefono];
GO
IF OBJECT_ID(N'[dbo].[TipoIdentificacion]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TipoIdentificacion];
GO
IF OBJECT_ID(N'[dbo].[Ubicacion]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Ubicacion];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Articulo'
CREATE TABLE [dbo].[Articulo] (
    [Codigo_Id] uniqueidentifier  NOT NULL,
    [Nombre] varchar(300)  NOT NULL,
    [Referencia] varchar(200)  NOT NULL,
    [Descripcion] varchar(max)  NOT NULL,
    [Marca] varchar(200)  NOT NULL,
    [ValorUnitario] int  NOT NULL,
    [PrecioIn] int  NOT NULL,
    [PrecioOut] int  NOT NULL,
    [Estado] bit  NULL,
    [Subcategoria_Id] uniqueidentifier  NOT NULL,
    [Usuario_Id] uniqueidentifier  NOT NULL,
    [Created_At] datetime  NOT NULL,
    [Updated_At] datetime  NULL
);
GO

-- Creating table 'Aula'
CREATE TABLE [dbo].[Aula] (
    [Codigo_Id] uniqueidentifier  NOT NULL,
    [CodigoAula] int  NOT NULL,
    [Nombre] varchar(300)  NOT NULL,
    [Sede_Id] uniqueidentifier  NULL,
    [Created_At] datetime  NOT NULL,
    [Updated_At] datetime  NULL
);
GO

-- Creating table 'Categoria'
CREATE TABLE [dbo].[Categoria] (
    [Codigo_Id] uniqueidentifier  NOT NULL,
    [Referencia] int  NOT NULL,
    [Nombre] varchar(200)  NOT NULL,
    [Estado] bit  NULL,
    [Created_At] datetime  NOT NULL,
    [Updated_At] datetime  NULL
);
GO

-- Creating table 'Curso'
CREATE TABLE [dbo].[Curso] (
    [Codigo_Id] uniqueidentifier  NOT NULL,
    [Nivel] int  NOT NULL,
    [Nombre] varchar(300)  NOT NULL,
    [Aula_Id] uniqueidentifier  NULL,
    [Created_At] datetime  NOT NULL,
    [Updated_At] datetime  NULL
);
GO

-- Creating table 'Departamento'
CREATE TABLE [dbo].[Departamento] (
    [Codigo_Id] uniqueidentifier  NOT NULL,
    [Dane_Id] int  NOT NULL,
    [Nombre] varchar(300)  NOT NULL,
    [Created_At] datetime  NULL,
    [Updated_At] datetime  NULL
);
GO

-- Creating table 'Entidad'
CREATE TABLE [dbo].[Entidad] (
    [Codigo_Id] uniqueidentifier  NOT NULL,
    [Nit] int  NOT NULL,
    [Nombre] varchar(200)  NOT NULL,
    [CodigoEntidad] int  NOT NULL,
    [CodigoDane] int  NOT NULL,
    [Foto] nvarchar(max)  NULL,
    [FiniFiscal] datetime  NOT NULL,
    [FfinFiscal] datetime  NOT NULL,
    [Entidad_Padre] uniqueidentifier  NULL,
    [Ubicacion_Id] uniqueidentifier  NULL,
    [Created_At] datetime  NULL,
    [Updated_At] datetime  NULL
);
GO

-- Creating table 'Entidad_Persona_Rol'
CREATE TABLE [dbo].[Entidad_Persona_Rol] (
    [Rol_Id] uniqueidentifier  NOT NULL,
    [Persona_Id] uniqueidentifier  NOT NULL,
    [Entidad_Id] uniqueidentifier  NOT NULL,
    [Estado] bit  NULL,
    [Created_At] datetime  NULL,
    [Updated_At] datetime  NULL,
    [Usuario_Update] uniqueidentifier  NULL
);
GO

-- Creating table 'GenericError'
CREATE TABLE [dbo].[GenericError] (
    [codigo_id] int IDENTITY(1,1) NOT NULL,
    [Codigo] nvarchar(4)  NULL,
    [Message] nvarchar(max)  NULL
);
GO

-- Creating table 'Imagen'
CREATE TABLE [dbo].[Imagen] (
    [Codigo_Id] uniqueidentifier  NOT NULL,
    [Fuente_Id] uniqueidentifier  NOT NULL,
    [RutaImagen] varchar(20)  NOT NULL,
    [Created_At] datetime  NOT NULL,
    [Updated_At] datetime  NULL
);
GO

-- Creating table 'Licencia'
CREATE TABLE [dbo].[Licencia] (
    [Codigo_Id] uniqueidentifier  NOT NULL,
    [Entidad_Id] uniqueidentifier  NULL,
    [NumeroLicencia] int  NOT NULL,
    [FiniVigencia] datetime  NOT NULL,
    [FfinVigencia] datetime  NOT NULL,
    [Estado] bit  NULL,
    [CostoLicencia] varchar(20)  NOT NULL,
    [Created_At] datetime  NULL,
    [Updated_At] datetime  NULL,
    [IsTest] bit  NOT NULL
);
GO

-- Creating table 'Municipio'
CREATE TABLE [dbo].[Municipio] (
    [Codigo_Id] uniqueidentifier  NOT NULL,
    [Departamento_Id] int  NOT NULL,
    [Dane_Id] int  NOT NULL,
    [Nombre] varchar(200)  NOT NULL,
    [Created_At] datetime  NULL,
    [Updated_At] datetime  NULL
);
GO

-- Creating table 'Persona'
CREATE TABLE [dbo].[Persona] (
    [Codigo_Id] uniqueidentifier  NOT NULL,
    [NumeroIdentificacion] int  NOT NULL,
    [Nombres] varchar(300)  NOT NULL,
    [Apellidos] varchar(300)  NOT NULL,
    [Sexo] varchar(20)  NOT NULL,
    [FNacimiento] datetime  NOT NULL,
    [Nacionalidad] varchar(300)  NULL,
    [LibretaMilitar] varchar(300)  NULL,
    [TipoSangre] varchar(100)  NULL,
    [Estado_Persona] bit  NOT NULL,
    [Ubicacion_Id] uniqueidentifier  NOT NULL,
    [Foto] nvarchar(max)  NULL,
    [TipoIdentificacion_Id] uniqueidentifier  NOT NULL,
    [Created_At] datetime  NOT NULL,
    [Updated_At] datetime  NULL,
    [Login] nvarchar(20)  NULL,
    [Password] nvarchar(100)  NULL,
    [Email] nvarchar(50)  NULL,
    [Estado_Usuario] bit  NULL,
    [Cambiar_Clave] bit  NULL,
    [Usuario_Update] uniqueidentifier  NULL
);
GO

-- Creating table 'Poblado'
CREATE TABLE [dbo].[Poblado] (
    [Codigo_Id] uniqueidentifier  NOT NULL,
    [Municipio_Id] int  NOT NULL,
    [Poblado_Id] int  NOT NULL,
    [Nombre] varchar(200)  NOT NULL,
    [Tipo] varchar(50)  NOT NULL,
    [Created_At] datetime  NULL,
    [Updated_At] datetime  NULL
);
GO

-- Creating table 'Rol'
CREATE TABLE [dbo].[Rol] (
    [Codigo_Id] uniqueidentifier  NOT NULL,
    [Nombre] varchar(200)  NOT NULL,
    [Estado] bit  NULL,
    [Descripcion] varchar(200)  NOT NULL,
    [Created_At] datetime  NULL,
    [Updated_At] datetime  NULL
);
GO

-- Creating table 'Subcategoria'
CREATE TABLE [dbo].[Subcategoria] (
    [Codigo_Id] uniqueidentifier  NOT NULL,
    [Referencia] int  NOT NULL,
    [Nombre] varchar(200)  NOT NULL,
    [Descripcion] varchar(max)  NOT NULL,
    [Estado] bit  NULL,
    [Categoria_Id] uniqueidentifier  NOT NULL,
    [Created_At] datetime  NOT NULL,
    [Updated_At] datetime  NULL
);
GO

-- Creating table 'Telefono'
CREATE TABLE [dbo].[Telefono] (
    [Codigo_Id] uniqueidentifier  NOT NULL,
    [Persona_Id] uniqueidentifier  NULL,
    [Entidad_Id] uniqueidentifier  NULL,
    [Numero] int  NOT NULL,
    [Tipo] varchar(20)  NOT NULL,
    [Created_At] datetime  NULL,
    [Updated_At] datetime  NULL
);
GO

-- Creating table 'TipoIdentificacion'
CREATE TABLE [dbo].[TipoIdentificacion] (
    [Codigo_Id] uniqueidentifier  NOT NULL,
    [Nombre] varchar(200)  NOT NULL,
    [Created_At] datetime  NOT NULL,
    [Updated_At] datetime  NULL
);
GO

-- Creating table 'Ubicacion'
CREATE TABLE [dbo].[Ubicacion] (
    [Codigo_Id] uniqueidentifier  NOT NULL,
    [Poblado_Id] int  NOT NULL,
    [Direccion] varchar(200)  NOT NULL,
    [Latitud] varchar(50)  NULL,
    [Longitud] varchar(50)  NULL,
    [Created_At] datetime  NULL,
    [Updated_At] datetime  NULL,
    [Usuario_Update] uniqueidentifier  NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Codigo_Id] in table 'Articulo'
ALTER TABLE [dbo].[Articulo]
ADD CONSTRAINT [PK_Articulo]
    PRIMARY KEY CLUSTERED ([Codigo_Id] ASC);
GO

-- Creating primary key on [Codigo_Id] in table 'Aula'
ALTER TABLE [dbo].[Aula]
ADD CONSTRAINT [PK_Aula]
    PRIMARY KEY CLUSTERED ([Codigo_Id] ASC);
GO

-- Creating primary key on [Codigo_Id] in table 'Categoria'
ALTER TABLE [dbo].[Categoria]
ADD CONSTRAINT [PK_Categoria]
    PRIMARY KEY CLUSTERED ([Codigo_Id] ASC);
GO

-- Creating primary key on [Codigo_Id] in table 'Curso'
ALTER TABLE [dbo].[Curso]
ADD CONSTRAINT [PK_Curso]
    PRIMARY KEY CLUSTERED ([Codigo_Id] ASC);
GO

-- Creating primary key on [Dane_Id] in table 'Departamento'
ALTER TABLE [dbo].[Departamento]
ADD CONSTRAINT [PK_Departamento]
    PRIMARY KEY CLUSTERED ([Dane_Id] ASC);
GO

-- Creating primary key on [Codigo_Id] in table 'Entidad'
ALTER TABLE [dbo].[Entidad]
ADD CONSTRAINT [PK_Entidad]
    PRIMARY KEY CLUSTERED ([Codigo_Id] ASC);
GO

-- Creating primary key on [Rol_Id], [Persona_Id], [Entidad_Id] in table 'Entidad_Persona_Rol'
ALTER TABLE [dbo].[Entidad_Persona_Rol]
ADD CONSTRAINT [PK_Entidad_Persona_Rol]
    PRIMARY KEY CLUSTERED ([Rol_Id], [Persona_Id], [Entidad_Id] ASC);
GO

-- Creating primary key on [codigo_id] in table 'GenericError'
ALTER TABLE [dbo].[GenericError]
ADD CONSTRAINT [PK_GenericError]
    PRIMARY KEY CLUSTERED ([codigo_id] ASC);
GO

-- Creating primary key on [Codigo_Id] in table 'Imagen'
ALTER TABLE [dbo].[Imagen]
ADD CONSTRAINT [PK_Imagen]
    PRIMARY KEY CLUSTERED ([Codigo_Id] ASC);
GO

-- Creating primary key on [Codigo_Id] in table 'Licencia'
ALTER TABLE [dbo].[Licencia]
ADD CONSTRAINT [PK_Licencia]
    PRIMARY KEY CLUSTERED ([Codigo_Id] ASC);
GO

-- Creating primary key on [Dane_Id] in table 'Municipio'
ALTER TABLE [dbo].[Municipio]
ADD CONSTRAINT [PK_Municipio]
    PRIMARY KEY CLUSTERED ([Dane_Id] ASC);
GO

-- Creating primary key on [Codigo_Id] in table 'Persona'
ALTER TABLE [dbo].[Persona]
ADD CONSTRAINT [PK_Persona]
    PRIMARY KEY CLUSTERED ([Codigo_Id] ASC);
GO

-- Creating primary key on [Poblado_Id] in table 'Poblado'
ALTER TABLE [dbo].[Poblado]
ADD CONSTRAINT [PK_Poblado]
    PRIMARY KEY CLUSTERED ([Poblado_Id] ASC);
GO

-- Creating primary key on [Codigo_Id] in table 'Rol'
ALTER TABLE [dbo].[Rol]
ADD CONSTRAINT [PK_Rol]
    PRIMARY KEY CLUSTERED ([Codigo_Id] ASC);
GO

-- Creating primary key on [Codigo_Id] in table 'Subcategoria'
ALTER TABLE [dbo].[Subcategoria]
ADD CONSTRAINT [PK_Subcategoria]
    PRIMARY KEY CLUSTERED ([Codigo_Id] ASC);
GO

-- Creating primary key on [Codigo_Id] in table 'Telefono'
ALTER TABLE [dbo].[Telefono]
ADD CONSTRAINT [PK_Telefono]
    PRIMARY KEY CLUSTERED ([Codigo_Id] ASC);
GO

-- Creating primary key on [Codigo_Id] in table 'TipoIdentificacion'
ALTER TABLE [dbo].[TipoIdentificacion]
ADD CONSTRAINT [PK_TipoIdentificacion]
    PRIMARY KEY CLUSTERED ([Codigo_Id] ASC);
GO

-- Creating primary key on [Codigo_Id] in table 'Ubicacion'
ALTER TABLE [dbo].[Ubicacion]
ADD CONSTRAINT [PK_Ubicacion]
    PRIMARY KEY CLUSTERED ([Codigo_Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [Fuente_Id] in table 'Imagen'
ALTER TABLE [dbo].[Imagen]
ADD CONSTRAINT [FK_IArticulo]
    FOREIGN KEY ([Fuente_Id])
    REFERENCES [dbo].[Articulo]
        ([Codigo_Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_IArticulo'
CREATE INDEX [IX_FK_IArticulo]
ON [dbo].[Imagen]
    ([Fuente_Id]);
GO

-- Creating foreign key on [Subcategoria_Id] in table 'Articulo'
ALTER TABLE [dbo].[Articulo]
ADD CONSTRAINT [FK_USubcategoria]
    FOREIGN KEY ([Subcategoria_Id])
    REFERENCES [dbo].[Subcategoria]
        ([Codigo_Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_USubcategoria'
CREATE INDEX [IX_FK_USubcategoria]
ON [dbo].[Articulo]
    ([Subcategoria_Id]);
GO

-- Creating foreign key on [Aula_Id] in table 'Curso'
ALTER TABLE [dbo].[Curso]
ADD CONSTRAINT [FK_CAula]
    FOREIGN KEY ([Aula_Id])
    REFERENCES [dbo].[Aula]
        ([Codigo_Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_CAula'
CREATE INDEX [IX_FK_CAula]
ON [dbo].[Curso]
    ([Aula_Id]);
GO

-- Creating foreign key on [Categoria_Id] in table 'Subcategoria'
ALTER TABLE [dbo].[Subcategoria]
ADD CONSTRAINT [FK_CSubcategoria]
    FOREIGN KEY ([Categoria_Id])
    REFERENCES [dbo].[Categoria]
        ([Codigo_Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_CSubcategoria'
CREATE INDEX [IX_FK_CSubcategoria]
ON [dbo].[Subcategoria]
    ([Categoria_Id]);
GO

-- Creating foreign key on [Departamento_Id] in table 'Municipio'
ALTER TABLE [dbo].[Municipio]
ADD CONSTRAINT [FK_MDepartamento]
    FOREIGN KEY ([Departamento_Id])
    REFERENCES [dbo].[Departamento]
        ([Dane_Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_MDepartamento'
CREATE INDEX [IX_FK_MDepartamento]
ON [dbo].[Municipio]
    ([Departamento_Id]);
GO

-- Creating foreign key on [Entidad_Padre] in table 'Entidad'
ALTER TABLE [dbo].[Entidad]
ADD CONSTRAINT [FK_EEntidad_Padre]
    FOREIGN KEY ([Entidad_Padre])
    REFERENCES [dbo].[Entidad]
        ([Codigo_Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_EEntidad_Padre'
CREATE INDEX [IX_FK_EEntidad_Padre]
ON [dbo].[Entidad]
    ([Entidad_Padre]);
GO

-- Creating foreign key on [Ubicacion_Id] in table 'Entidad'
ALTER TABLE [dbo].[Entidad]
ADD CONSTRAINT [FK_EUbicacion]
    FOREIGN KEY ([Ubicacion_Id])
    REFERENCES [dbo].[Ubicacion]
        ([Codigo_Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_EUbicacion'
CREATE INDEX [IX_FK_EUbicacion]
ON [dbo].[Entidad]
    ([Ubicacion_Id]);
GO

-- Creating foreign key on [Entidad_Id] in table 'Licencia'
ALTER TABLE [dbo].[Licencia]
ADD CONSTRAINT [FK_LEntidad]
    FOREIGN KEY ([Entidad_Id])
    REFERENCES [dbo].[Entidad]
        ([Codigo_Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_LEntidad'
CREATE INDEX [IX_FK_LEntidad]
ON [dbo].[Licencia]
    ([Entidad_Id]);
GO

-- Creating foreign key on [Entidad_Id] in table 'Entidad_Persona_Rol'
ALTER TABLE [dbo].[Entidad_Persona_Rol]
ADD CONSTRAINT [FK_Persona_Rol_Entidad]
    FOREIGN KEY ([Entidad_Id])
    REFERENCES [dbo].[Entidad]
        ([Codigo_Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Persona_Rol_Entidad'
CREATE INDEX [IX_FK_Persona_Rol_Entidad]
ON [dbo].[Entidad_Persona_Rol]
    ([Entidad_Id]);
GO

-- Creating foreign key on [Entidad_Id] in table 'Telefono'
ALTER TABLE [dbo].[Telefono]
ADD CONSTRAINT [FK_TEntidad]
    FOREIGN KEY ([Entidad_Id])
    REFERENCES [dbo].[Entidad]
        ([Codigo_Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_TEntidad'
CREATE INDEX [IX_FK_TEntidad]
ON [dbo].[Telefono]
    ([Entidad_Id]);
GO

-- Creating foreign key on [Persona_Id] in table 'Entidad_Persona_Rol'
ALTER TABLE [dbo].[Entidad_Persona_Rol]
ADD CONSTRAINT [FK_Usuario_Rol_Persona1]
    FOREIGN KEY ([Persona_Id])
    REFERENCES [dbo].[Persona]
        ([Codigo_Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Usuario_Rol_Persona1'
CREATE INDEX [IX_FK_Usuario_Rol_Persona1]
ON [dbo].[Entidad_Persona_Rol]
    ([Persona_Id]);
GO

-- Creating foreign key on [Rol_Id] in table 'Entidad_Persona_Rol'
ALTER TABLE [dbo].[Entidad_Persona_Rol]
ADD CONSTRAINT [FK_Usuario_Rol_Rol]
    FOREIGN KEY ([Rol_Id])
    REFERENCES [dbo].[Rol]
        ([Codigo_Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Fuente_Id] in table 'Imagen'
ALTER TABLE [dbo].[Imagen]
ADD CONSTRAINT [FK_IPersona]
    FOREIGN KEY ([Fuente_Id])
    REFERENCES [dbo].[Persona]
        ([Codigo_Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_IPersona'
CREATE INDEX [IX_FK_IPersona]
ON [dbo].[Imagen]
    ([Fuente_Id]);
GO

-- Creating foreign key on [Municipio_Id] in table 'Poblado'
ALTER TABLE [dbo].[Poblado]
ADD CONSTRAINT [FK_PMunicipio]
    FOREIGN KEY ([Municipio_Id])
    REFERENCES [dbo].[Municipio]
        ([Dane_Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_PMunicipio'
CREATE INDEX [IX_FK_PMunicipio]
ON [dbo].[Poblado]
    ([Municipio_Id]);
GO

-- Creating foreign key on [TipoIdentificacion_Id] in table 'Persona'
ALTER TABLE [dbo].[Persona]
ADD CONSTRAINT [FK_PTipoIdentificacion]
    FOREIGN KEY ([TipoIdentificacion_Id])
    REFERENCES [dbo].[TipoIdentificacion]
        ([Codigo_Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_PTipoIdentificacion'
CREATE INDEX [IX_FK_PTipoIdentificacion]
ON [dbo].[Persona]
    ([TipoIdentificacion_Id]);
GO

-- Creating foreign key on [Ubicacion_Id] in table 'Persona'
ALTER TABLE [dbo].[Persona]
ADD CONSTRAINT [FK_PUbicacion]
    FOREIGN KEY ([Ubicacion_Id])
    REFERENCES [dbo].[Ubicacion]
        ([Codigo_Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_PUbicacion'
CREATE INDEX [IX_FK_PUbicacion]
ON [dbo].[Persona]
    ([Ubicacion_Id]);
GO

-- Creating foreign key on [Persona_Id] in table 'Telefono'
ALTER TABLE [dbo].[Telefono]
ADD CONSTRAINT [FK_Telefono_Persona]
    FOREIGN KEY ([Persona_Id])
    REFERENCES [dbo].[Persona]
        ([Codigo_Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Telefono_Persona'
CREATE INDEX [IX_FK_Telefono_Persona]
ON [dbo].[Telefono]
    ([Persona_Id]);
GO

-- Creating foreign key on [Poblado_Id] in table 'Ubicacion'
ALTER TABLE [dbo].[Ubicacion]
ADD CONSTRAINT [FK_UPoblado]
    FOREIGN KEY ([Poblado_Id])
    REFERENCES [dbo].[Poblado]
        ([Poblado_Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_UPoblado'
CREATE INDEX [IX_FK_UPoblado]
ON [dbo].[Ubicacion]
    ([Poblado_Id]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------