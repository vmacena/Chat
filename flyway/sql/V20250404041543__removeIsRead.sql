start transaction;


do $ef$
begin
    if not exists(select 1 from "__EFMigrationsHistory" where "migrationid" = '20250404041543_removeIsRead') then
    alter table "messages" drop column "isread";
    end if;
end $ef$;

do $ef$
begin
    if not exists(select 1 from "__EFMigrationsHistory" where "migrationid" = '20250404041543_removeIsRead') then
    insert into "__EFMigrationsHistory" ("migrationid", "productversion")
    values ('20250404041543_removeIsRead', '8.0.0');
    end if;
end $ef$;
commit;

