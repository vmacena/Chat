create table if not exists "__EFMigrationsHistory" (
    "migrationid" character varying(150) not null,
    "productversion" character varying(32) not null,
    constraint "PK___EFMigrationsHistory" primary key ("migrationid")
);

start transaction;


do $ef$
begin
    if not exists(select 1 from "__EFMigrationsHistory" where "migrationid" = '20250304162442_first_migration') then
    create table "users" (
        "id" uuid not null,
        "name" text not null,
        "email" text not null,
        "idshare" text not null,
        "password" text not null,
        constraint "PK_Users" primary key ("id")
    );
    end if;
end $ef$;

do $ef$
begin
    if not exists(select 1 from "__EFMigrationsHistory" where "migrationid" = '20250304162442_first_migration') then
    create table "contacts" (
        "id" uuid not null,
        "userid" uuid not null,
        "contactid" uuid not null,
        constraint "PK_Contacts" primary key ("id"),
        constraint "FK_Contacts_Users_ContactId" foreign key ("contactid") references "users" ("id") on delete restrict,
        constraint "FK_Contacts_Users_UserId" foreign key ("userid") references "users" ("id") on delete restrict
    );
    end if;
end $ef$;

do $ef$
begin
    if not exists(select 1 from "__EFMigrationsHistory" where "migrationid" = '20250304162442_first_migration') then
    create table "messages" (
        "id" uuid not null,
        "senderid" uuid not null,
        "receiverid" uuid not null,
        "content" text,
        "timestamp" timestamp with time zone not null,
        "isread" boolean not null,
        constraint "PK_Messages" primary key ("id"),
        constraint "FK_Messages_Users_ReceiverId" foreign key ("receiverid") references "users" ("id") on delete restrict,
        constraint "FK_Messages_Users_SenderId" foreign key ("senderid") references "users" ("id") on delete restrict
    );
    end if;
end $ef$;

do $ef$
begin
    if not exists(select 1 from "__EFMigrationsHistory" where "migrationid" = '20250304162442_first_migration') then
    create table "uploadfiles" (
        "id" uuid not null,
        "messageid" uuid not null,
        "type" text,
        "url" text,
        "size" bigint not null,
        constraint "PK_UploadFiles" primary key ("id"),
        constraint "FK_UploadFiles_Messages_MessageId" foreign key ("messageid") references "messages" ("id") on delete cascade
    );
    end if;
end $ef$;

do $ef$
begin
    if not exists(select 1 from "__EFMigrationsHistory" where "migrationid" = '20250304162442_first_migration') then
    create index "IX_Contacts_ContactId" on "contacts" ("contactid");
    end if;
end $ef$;

do $ef$
begin
    if not exists(select 1 from "__EFMigrationsHistory" where "migrationid" = '20250304162442_first_migration') then
    create index "IX_Contacts_UserId" on "contacts" ("userid");
    end if;
end $ef$;

do $ef$
begin
    if not exists(select 1 from "__EFMigrationsHistory" where "migrationid" = '20250304162442_first_migration') then
    create index "IX_Messages_ReceiverId" on "messages" ("receiverid");
    end if;
end $ef$;

do $ef$
begin
    if not exists(select 1 from "__EFMigrationsHistory" where "migrationid" = '20250304162442_first_migration') then
    create index "IX_Messages_SenderId" on "messages" ("senderid");
    end if;
end $ef$;

do $ef$
begin
    if not exists(select 1 from "__EFMigrationsHistory" where "migrationid" = '20250304162442_first_migration') then
    create index "IX_UploadFiles_MessageId" on "uploadfiles" ("messageid");
    end if;
end $ef$;

do $ef$
begin
    if not exists(select 1 from "__EFMigrationsHistory" where "migrationid" = '20250304162442_first_migration') then
    insert into "__EFMigrationsHistory" ("migrationid", "productversion")
    values ('20250304162442_first_migration', '8.0.0');
    end if;
end $ef$;
commit;

