# HDF5 2.2 API coverage

This inventory compares the pinned HDF5 2.2.0 core and high-level `*public.h` function declarations with the shared C# bindings used by both managed targets. Regenerate it with `Build/apiCoverage.ps1`. Each native symbol is counted once; managed overloads do not increase coverage. A binding means a declaration exists, not that every combination of arguments has been tested.

Macros, exported datatype/property globals, VFD-specific headers and VOL connector-author interfaces are outside these function counts. Conditional and deprecated declarations are included, even when the packaged serial native build does not export them. See [API_PLAN.md](API_PLAN.md) for priorities, scope and validation requirements.

| Group | Native functions | Bound | Gaps |
|---|---:|---:|---:|
| H5A | 43 | 26 | 17 |
| H5DO | 3 | 3 | 0 |
| H5D | 47 | 34 | 13 |
| H5DS | 11 | 11 | 0 |
| H5E | 31 | 23 | 8 |
| H5ES | 12 | 0 | 12 |
| H5FD | 1 | 0 | 1 |
| H5F | 52 | 43 | 9 |
| H5G | 31 | 10 | 21 |
| H5IM | 12 | 0 | 12 |
| H5I | 21 | 21 | 0 |
| H5LD | 3 | 0 | 3 |
| H5L | 32 | 26 | 6 |
| H5LT | 53 | 53 | 0 |
| H5M | 20 | 0 | 20 |
| H5O | 47 | 33 | 14 |
| H5PL | 9 | 9 | 0 |
| H5P | 225 | 207 | 18 |
| H5PT | 16 | 0 | 16 |
| H5 | 14 | 12 | 2 |
| H5R | 24 | 19 | 5 |
| H5S | 43 | 43 | 0 |
| H5TB | 19 | 0 | 19 |
| H5T | 73 | 66 | 7 |
| H5VL | 12 | 12 | 0 |
| H5Z | 2 | 2 | 0 |

## H5A

Native header: `H5Apublic.h`.

| Native function | C# coverage |
|---|---|
| `H5Aclose` | Bound |
| `H5Aclose_async` | Gap |
| `H5Acreate_async` | Gap |
| `H5Acreate_by_name` | Bound |
| `H5Acreate_by_name_async` | Gap |
| `H5Acreate1` | Gap |
| `H5Acreate2` | Bound |
| `H5Adelete` | Bound |
| `H5Adelete_by_idx` | Bound |
| `H5Adelete_by_name` | Bound |
| `H5Aexists` | Bound |
| `H5Aexists_async` | Gap |
| `H5Aexists_by_name` | Bound |
| `H5Aexists_by_name_async` | Gap |
| `H5Aget_create_plist` | Bound |
| `H5Aget_info` | Bound |
| `H5Aget_info_by_idx` | Bound |
| `H5Aget_info_by_name` | Bound |
| `H5Aget_name` | Bound |
| `H5Aget_name_by_idx` | Bound |
| `H5Aget_num_attrs` | Gap |
| `H5Aget_space` | Bound |
| `H5Aget_storage_size` | Bound |
| `H5Aget_type` | Bound |
| `H5Aiterate_by_name` | Bound |
| `H5Aiterate1` | Gap |
| `H5Aiterate2` | Bound |
| `H5Aopen` | Bound |
| `H5Aopen_async` | Gap |
| `H5Aopen_by_idx` | Bound |
| `H5Aopen_by_idx_async` | Gap |
| `H5Aopen_by_name` | Bound |
| `H5Aopen_by_name_async` | Gap |
| `H5Aopen_idx` | Gap |
| `H5Aopen_name` | Gap |
| `H5Aread` | Bound |
| `H5Aread_async` | Gap |
| `H5Arename` | Bound |
| `H5Arename_async` | Gap |
| `H5Arename_by_name` | Bound |
| `H5Arename_by_name_async` | Gap |
| `H5Awrite` | Bound |
| `H5Awrite_async` | Gap |

## H5DO

Native header: `H5DOpublic.h`.

| Native function | C# coverage |
|---|---|
| `H5DOappend` | Bound |
| `H5DOread_chunk` | Bound |
| `H5DOwrite_chunk` | Bound |

## H5D

Native header: `H5Dpublic.h`.

| Native function | C# coverage |
|---|---|
| `H5Dchunk_iter` | Bound |
| `H5Dclose` | Bound |
| `H5Dclose_async` | Gap |
| `H5Dcreate_anon` | Bound |
| `H5Dcreate_async` | Gap |
| `H5Dcreate1` | Gap |
| `H5Dcreate2` | Bound |
| `H5Ddebug` | Gap |
| `H5Dextend` | Gap |
| `H5Dfill` | Bound |
| `H5Dflush` | Bound |
| `H5Dformat_convert` | Bound |
| `H5Dgather` | Bound |
| `H5Dget_access_plist` | Bound |
| `H5Dget_chunk_index_type` | Bound |
| `H5Dget_chunk_info` | Bound |
| `H5Dget_chunk_info_by_coord` | Bound |
| `H5Dget_chunk_storage_size` | Bound |
| `H5Dget_create_plist` | Bound |
| `H5Dget_num_chunks` | Bound |
| `H5Dget_offset` | Bound |
| `H5Dget_space` | Bound |
| `H5Dget_space_async` | Gap |
| `H5Dget_space_status` | Bound |
| `H5Dget_storage_size` | Bound |
| `H5Dget_type` | Bound |
| `H5Diterate` | Bound |
| `H5Dopen_async` | Gap |
| `H5Dopen1` | Gap |
| `H5Dopen2` | Bound |
| `H5Dread` | Bound |
| `H5Dread_async` | Gap |
| `H5Dread_chunk1` | Bound |
| `H5Dread_chunk2` | Bound |
| `H5Dread_multi` | Bound |
| `H5Dread_multi_async` | Gap |
| `H5Drefresh` | Bound |
| `H5Dscatter` | Bound |
| `H5Dset_extent` | Bound |
| `H5Dset_extent_async` | Gap |
| `H5Dvlen_get_buf_size` | Bound |
| `H5Dvlen_reclaim` | Bound |
| `H5Dwrite` | Bound |
| `H5Dwrite_async` | Gap |
| `H5Dwrite_chunk` | Bound |
| `H5Dwrite_multi` | Bound |
| `H5Dwrite_multi_async` | Gap |

## H5DS

Native header: `H5DSpublic.h`.

| Native function | C# coverage |
|---|---|
| `H5DSattach_scale` | Bound |
| `H5DSdetach_scale` | Bound |
| `H5DSget_label` | Bound |
| `H5DSget_num_scales` | Bound |
| `H5DSget_scale_name` | Bound |
| `H5DSis_attached` | Bound |
| `H5DSis_scale` | Bound |
| `H5DSiterate_scales` | Bound |
| `H5DSset_label` | Bound |
| `H5DSset_scale` | Bound |
| `H5DSwith_new_ref` | Bound |

## H5E

Native header: `H5Epublic.h`.

| Native function | C# coverage |
|---|---|
| `H5Eappend_stack` | Bound |
| `H5Eauto_is_v2` | Bound |
| `H5Eclear1` | Gap |
| `H5Eclear2` | Bound |
| `H5Eclose_msg` | Bound |
| `H5Eclose_stack` | Bound |
| `H5Ecreate_msg` | Bound |
| `H5Ecreate_stack` | Bound |
| `H5Eget_auto1` | Gap |
| `H5Eget_auto2` | Bound |
| `H5Eget_class_name` | Bound |
| `H5Eget_current_stack` | Bound |
| `H5Eget_major` | Gap |
| `H5Eget_minor` | Gap |
| `H5Eget_msg` | Bound |
| `H5Eget_num` | Bound |
| `H5Eis_paused` | Bound |
| `H5Epause_stack` | Bound |
| `H5Epop` | Bound |
| `H5Eprint1` | Gap |
| `H5Eprint2` | Bound |
| `H5Epush1` | Gap |
| `H5Epush2` | Bound |
| `H5Eregister_class` | Bound |
| `H5Eresume_stack` | Bound |
| `H5Eset_auto1` | Gap |
| `H5Eset_auto2` | Bound |
| `H5Eset_current_stack` | Bound |
| `H5Eunregister_class` | Bound |
| `H5Ewalk1` | Gap |
| `H5Ewalk2` | Bound |

## H5ES

Native header: `H5ESpublic.h`.

| Native function | C# coverage |
|---|---|
| `H5EScancel` | Gap |
| `H5ESclose` | Gap |
| `H5EScreate` | Gap |
| `H5ESfree_err_info` | Gap |
| `H5ESget_count` | Gap |
| `H5ESget_err_count` | Gap |
| `H5ESget_err_info` | Gap |
| `H5ESget_err_status` | Gap |
| `H5ESget_op_counter` | Gap |
| `H5ESregister_complete_func` | Gap |
| `H5ESregister_insert_func` | Gap |
| `H5ESwait` | Gap |

## H5FD

Native header: `H5FDpublic.h`.

| Native function | C# coverage |
|---|---|
| `H5FDdriver_query` | Gap |

## H5F

Native header: `H5Fpublic.h`.

| Native function | C# coverage |
|---|---|
| `H5Fclear_elink_file_cache` | Bound |
| `H5Fclose` | Bound |
| `H5Fclose_async` | Gap |
| `H5Fcreate` | Bound |
| `H5Fcreate_async` | Gap |
| `H5Fdelete` | Bound |
| `H5Fflush` | Bound |
| `H5Fflush_async` | Gap |
| `H5Fformat_convert` | Bound |
| `H5Fget_access_plist` | Bound |
| `H5Fget_create_plist` | Bound |
| `H5Fget_dset_no_attrs_hint` | Bound |
| `H5Fget_eoa` | Bound |
| `H5Fget_file_image` | Bound |
| `H5Fget_fileno` | Bound |
| `H5Fget_filesize` | Bound |
| `H5Fget_free_sections` | Bound |
| `H5Fget_freespace` | Bound |
| `H5Fget_info1` | Gap |
| `H5Fget_info2` | Bound |
| `H5Fget_intent` | Bound |
| `H5Fget_mdc_config` | Bound |
| `H5Fget_mdc_hit_rate` | Bound |
| `H5Fget_mdc_image_info` | Bound |
| `H5Fget_mdc_logging_status` | Bound |
| `H5Fget_mdc_size` | Bound |
| `H5Fget_metadata_read_retry_info` | Bound |
| `H5Fget_mpi_atomicity` | Unavailable in packaged serial build |
| `H5Fget_name` | Bound |
| `H5Fget_obj_count` | Bound |
| `H5Fget_obj_ids` | Bound |
| `H5Fget_page_buffering_stats` | Bound |
| `H5Fget_vfd_handle` | Bound |
| `H5Fincrement_filesize` | Bound |
| `H5Fis_accessible` | Bound |
| `H5Fis_hdf5` | Bound |
| `H5Fmount` | Bound |
| `H5Fopen` | Bound |
| `H5Fopen_async` | Gap |
| `H5Freopen` | Bound |
| `H5Freopen_async` | Gap |
| `H5Freset_mdc_hit_rate_stats` | Bound |
| `H5Freset_page_buffering_stats` | Bound |
| `H5Fset_dset_no_attrs_hint` | Bound |
| `H5Fset_latest_format` | Gap |
| `H5Fset_libver_bounds` | Bound |
| `H5Fset_mdc_config` | Bound |
| `H5Fset_mpi_atomicity` | Unavailable in packaged serial build |
| `H5Fstart_mdc_logging` | Bound |
| `H5Fstart_swmr_write` | Bound |
| `H5Fstop_mdc_logging` | Bound |
| `H5Funmount` | Bound |

## H5G

Native header: `H5Gpublic.h`.

| Native function | C# coverage |
|---|---|
| `H5Gclose` | Bound |
| `H5Gclose_async` | Gap |
| `H5Gcreate_anon` | Bound |
| `H5Gcreate_async` | Gap |
| `H5Gcreate1` | Gap |
| `H5Gcreate2` | Bound |
| `H5Gflush` | Bound |
| `H5Gget_comment` | Gap |
| `H5Gget_create_plist` | Bound |
| `H5Gget_info` | Bound |
| `H5Gget_info_async` | Gap |
| `H5Gget_info_by_idx` | Bound |
| `H5Gget_info_by_idx_async` | Gap |
| `H5Gget_info_by_name` | Bound |
| `H5Gget_info_by_name_async` | Gap |
| `H5Gget_linkval` | Gap |
| `H5Gget_num_objs` | Gap |
| `H5Gget_objinfo` | Gap |
| `H5Gget_objname_by_idx` | Gap |
| `H5Gget_objtype_by_idx` | Gap |
| `H5Giterate` | Gap |
| `H5Glink` | Gap |
| `H5Glink2` | Gap |
| `H5Gmove` | Gap |
| `H5Gmove2` | Gap |
| `H5Gopen_async` | Gap |
| `H5Gopen1` | Gap |
| `H5Gopen2` | Bound |
| `H5Grefresh` | Bound |
| `H5Gset_comment` | Gap |
| `H5Gunlink` | Gap |

## H5IM

Native header: `H5IMpublic.h`.

| Native function | C# coverage |
|---|---|
| `H5IMget_image_info` | Gap |
| `H5IMget_npalettes` | Gap |
| `H5IMget_palette` | Gap |
| `H5IMget_palette_info` | Gap |
| `H5IMis_image` | Gap |
| `H5IMis_palette` | Gap |
| `H5IMlink_palette` | Gap |
| `H5IMmake_image_24bit` | Gap |
| `H5IMmake_image_8bit` | Gap |
| `H5IMmake_palette` | Gap |
| `H5IMread_image` | Gap |
| `H5IMunlink_palette` | Gap |

## H5I

Native header: `H5Ipublic.h`.

| Native function | C# coverage |
|---|---|
| `H5Iclear_type` | Bound |
| `H5Idec_ref` | Bound |
| `H5Idec_type_ref` | Bound |
| `H5Idestroy_type` | Bound |
| `H5Iget_file_id` | Bound |
| `H5Iget_name` | Bound |
| `H5Iget_ref` | Bound |
| `H5Iget_type` | Bound |
| `H5Iget_type_ref` | Bound |
| `H5Iinc_ref` | Bound |
| `H5Iinc_type_ref` | Bound |
| `H5Iis_valid` | Bound |
| `H5Iiterate` | Bound |
| `H5Inmembers` | Bound |
| `H5Iobject_verify` | Bound |
| `H5Iregister` | Bound |
| `H5Iregister_type1` | Bound |
| `H5Iregister_type2` | Bound |
| `H5Iremove_verify` | Bound |
| `H5Isearch` | Bound |
| `H5Itype_exists` | Bound |

## H5LD

Native header: `H5LDpublic.h`.

| Native function | C# coverage |
|---|---|
| `H5LDget_dset_dims` | Gap |
| `H5LDget_dset_elmts` | Gap |
| `H5LDget_dset_type_size` | Gap |

## H5L

Native header: `H5Lpublic.h`.

| Native function | C# coverage |
|---|---|
| `H5Lcopy` | Bound |
| `H5Lcreate_external` | Bound |
| `H5Lcreate_hard` | Bound |
| `H5Lcreate_hard_async` | Gap |
| `H5Lcreate_soft` | Bound |
| `H5Lcreate_soft_async` | Gap |
| `H5Lcreate_ud` | Bound |
| `H5Ldelete` | Bound |
| `H5Ldelete_async` | Gap |
| `H5Ldelete_by_idx` | Bound |
| `H5Ldelete_by_idx_async` | Gap |
| `H5Lexists` | Bound |
| `H5Lexists_async` | Gap |
| `H5Lget_info_by_idx1` | Bound |
| `H5Lget_info_by_idx2` | Bound |
| `H5Lget_info1` | Bound |
| `H5Lget_info2` | Bound |
| `H5Lget_name_by_idx` | Bound |
| `H5Lget_val` | Bound |
| `H5Lget_val_by_idx` | Bound |
| `H5Lis_registered` | Bound |
| `H5Literate_async` | Gap |
| `H5Literate_by_name1` | Bound |
| `H5Literate_by_name2` | Bound |
| `H5Literate1` | Bound |
| `H5Literate2` | Bound |
| `H5Lmove` | Bound |
| `H5Lunpack_elink_val` | Bound |
| `H5Lvisit_by_name1` | Bound |
| `H5Lvisit_by_name2` | Bound |
| `H5Lvisit1` | Bound |
| `H5Lvisit2` | Bound |

## H5LT

Native header: `H5LTpublic.h`.

| Native function | C# coverage |
|---|---|
| `H5LTdtype_to_text` | Bound |
| `H5LTfind_attribute` | Bound |
| `H5LTfind_dataset` | Bound |
| `H5LTget_attribute` | Bound |
| `H5LTget_attribute_char` | Bound |
| `H5LTget_attribute_double` | Bound |
| `H5LTget_attribute_float` | Bound |
| `H5LTget_attribute_info` | Bound |
| `H5LTget_attribute_int` | Bound |
| `H5LTget_attribute_long` | Bound |
| `H5LTget_attribute_long_long` | Bound |
| `H5LTget_attribute_ndims` | Bound |
| `H5LTget_attribute_short` | Bound |
| `H5LTget_attribute_string` | Bound |
| `H5LTget_attribute_uchar` | Bound |
| `H5LTget_attribute_uint` | Bound |
| `H5LTget_attribute_ullong` | Bound |
| `H5LTget_attribute_ulong` | Bound |
| `H5LTget_attribute_ushort` | Bound |
| `H5LTget_dataset_info` | Bound |
| `H5LTget_dataset_ndims` | Bound |
| `H5LTmake_dataset` | Bound |
| `H5LTmake_dataset_char` | Bound |
| `H5LTmake_dataset_double` | Bound |
| `H5LTmake_dataset_float` | Bound |
| `H5LTmake_dataset_int` | Bound |
| `H5LTmake_dataset_long` | Bound |
| `H5LTmake_dataset_short` | Bound |
| `H5LTmake_dataset_string` | Bound |
| `H5LTopen_file_image` | Bound |
| `H5LTpath_valid` | Bound |
| `H5LTread_dataset` | Bound |
| `H5LTread_dataset_char` | Bound |
| `H5LTread_dataset_double` | Bound |
| `H5LTread_dataset_float` | Bound |
| `H5LTread_dataset_int` | Bound |
| `H5LTread_dataset_long` | Bound |
| `H5LTread_dataset_short` | Bound |
| `H5LTread_dataset_string` | Bound |
| `H5LTset_attribute_char` | Bound |
| `H5LTset_attribute_double` | Bound |
| `H5LTset_attribute_float` | Bound |
| `H5LTset_attribute_int` | Bound |
| `H5LTset_attribute_long` | Bound |
| `H5LTset_attribute_long_long` | Bound |
| `H5LTset_attribute_short` | Bound |
| `H5LTset_attribute_string` | Bound |
| `H5LTset_attribute_uchar` | Bound |
| `H5LTset_attribute_uint` | Bound |
| `H5LTset_attribute_ullong` | Bound |
| `H5LTset_attribute_ulong` | Bound |
| `H5LTset_attribute_ushort` | Bound |
| `H5LTtext_to_dtype` | Bound |

## H5M

Native header: `H5Mpublic.h`.

| Native function | C# coverage |
|---|---|
| `H5Mclose` | Unavailable in packaged serial build |
| `H5Mclose_async` | Unavailable in packaged serial build |
| `H5Mcreate` | Unavailable in packaged serial build |
| `H5Mcreate_anon` | Unavailable in packaged serial build |
| `H5Mcreate_async` | Unavailable in packaged serial build |
| `H5Mdelete` | Unavailable in packaged serial build |
| `H5Mexists` | Unavailable in packaged serial build |
| `H5Mget` | Unavailable in packaged serial build |
| `H5Mget_access_plist` | Unavailable in packaged serial build |
| `H5Mget_async` | Unavailable in packaged serial build |
| `H5Mget_count` | Unavailable in packaged serial build |
| `H5Mget_create_plist` | Unavailable in packaged serial build |
| `H5Mget_key_type` | Unavailable in packaged serial build |
| `H5Mget_val_type` | Unavailable in packaged serial build |
| `H5Miterate` | Unavailable in packaged serial build |
| `H5Miterate_by_name` | Unavailable in packaged serial build |
| `H5Mopen` | Unavailable in packaged serial build |
| `H5Mopen_async` | Unavailable in packaged serial build |
| `H5Mput` | Unavailable in packaged serial build |
| `H5Mput_async` | Unavailable in packaged serial build |

## H5O

Native header: `H5Opublic.h`.

| Native function | C# coverage |
|---|---|
| `H5Oare_mdc_flushes_disabled` | Bound |
| `H5Oclose` | Bound |
| `H5Oclose_async` | Gap |
| `H5Ocopy` | Bound |
| `H5Ocopy_async` | Gap |
| `H5Odecr_refcount` | Bound |
| `H5Odisable_mdc_flushes` | Bound |
| `H5Oenable_mdc_flushes` | Bound |
| `H5Oexists_by_name` | Bound |
| `H5Oflush` | Bound |
| `H5Oflush_async` | Gap |
| `H5Oget_comment` | Bound |
| `H5Oget_comment_by_name` | Bound |
| `H5Oget_info_by_idx1` | Bound |
| `H5Oget_info_by_idx2` | Gap |
| `H5Oget_info_by_idx3` | Bound |
| `H5Oget_info_by_name_async` | Gap |
| `H5Oget_info_by_name1` | Bound |
| `H5Oget_info_by_name2` | Gap |
| `H5Oget_info_by_name3` | Bound |
| `H5Oget_info1` | Bound |
| `H5Oget_info2` | Gap |
| `H5Oget_info3` | Bound |
| `H5Oget_native_info` | Bound |
| `H5Oget_native_info_by_idx` | Bound |
| `H5Oget_native_info_by_name` | Bound |
| `H5Oincr_refcount` | Bound |
| `H5Olink` | Bound |
| `H5Oopen` | Bound |
| `H5Oopen_async` | Gap |
| `H5Oopen_by_addr` | Bound |
| `H5Oopen_by_idx` | Bound |
| `H5Oopen_by_idx_async` | Gap |
| `H5Oopen_by_token` | Bound |
| `H5Orefresh` | Bound |
| `H5Orefresh_async` | Gap |
| `H5Oset_comment` | Gap |
| `H5Oset_comment_by_name` | Gap |
| `H5Otoken_cmp` | Bound |
| `H5Otoken_from_str` | Bound |
| `H5Otoken_to_str` | Bound |
| `H5Ovisit_by_name1` | Bound |
| `H5Ovisit_by_name2` | Gap |
| `H5Ovisit_by_name3` | Bound |
| `H5Ovisit1` | Bound |
| `H5Ovisit2` | Gap |
| `H5Ovisit3` | Bound |

## H5PL

Native header: `H5PLpublic.h`.

| Native function | C# coverage |
|---|---|
| `H5PLappend` | Bound |
| `H5PLget` | Bound |
| `H5PLget_loading_state` | Bound |
| `H5PLinsert` | Bound |
| `H5PLprepend` | Bound |
| `H5PLremove` | Bound |
| `H5PLreplace` | Bound |
| `H5PLset_loading_state` | Bound |
| `H5PLsize` | Bound |

## H5P

Native header: `H5Ppublic.h`.

| Native function | C# coverage |
|---|---|
| `H5Padd_merge_committed_dtype_path` | Bound |
| `H5Pall_filters_avail` | Bound |
| `H5Pclose` | Bound |
| `H5Pclose_class` | Bound |
| `H5Pcopy` | Bound |
| `H5Pcopy_prop` | Bound |
| `H5Pcreate` | Bound |
| `H5Pcreate_class` | Bound |
| `H5Pdecode` | Bound |
| `H5Pencode1` | Gap |
| `H5Pencode2` | Bound |
| `H5Pequal` | Bound |
| `H5Pexist` | Bound |
| `H5Pfill_value_defined` | Bound |
| `H5Pfree_merge_committed_dtype_paths` | Bound |
| `H5Pget` | Bound |
| `H5Pget_actual_selection_io_mode` | Bound |
| `H5Pget_alignment` | Bound |
| `H5Pget_all_coll_metadata_ops` | Unavailable in packaged serial build |
| `H5Pget_alloc_time` | Bound |
| `H5Pget_append_flush` | Bound |
| `H5Pget_attr_creation_order` | Bound |
| `H5Pget_attr_phase_change` | Bound |
| `H5Pget_btree_ratios` | Bound |
| `H5Pget_buffer` | Bound |
| `H5Pget_cache` | Bound |
| `H5Pget_char_encoding` | Bound |
| `H5Pget_chunk` | Bound |
| `H5Pget_chunk_cache` | Bound |
| `H5Pget_chunk_opts` | Bound |
| `H5Pget_class` | Bound |
| `H5Pget_class_name` | Bound |
| `H5Pget_class_parent` | Bound |
| `H5Pget_coll_metadata_write` | Unavailable in packaged serial build |
| `H5Pget_copy_object` | Bound |
| `H5Pget_core_write_tracking` | Bound |
| `H5Pget_create_intermediate_group` | Bound |
| `H5Pget_data_transform` | Bound |
| `H5Pget_driver` | Bound |
| `H5Pget_driver_config_str` | Bound |
| `H5Pget_driver_info` | Bound |
| `H5Pget_dset_no_attrs_hint` | Bound |
| `H5Pget_edc_check` | Bound |
| `H5Pget_efile_prefix` | Bound |
| `H5Pget_elink_acc_flags` | Bound |
| `H5Pget_elink_cb` | Bound |
| `H5Pget_elink_fapl` | Bound |
| `H5Pget_elink_file_cache_size` | Bound |
| `H5Pget_elink_prefix` | Bound |
| `H5Pget_est_link_info` | Bound |
| `H5Pget_evict_on_close` | Bound |
| `H5Pget_external` | Bound |
| `H5Pget_external_count` | Bound |
| `H5Pget_family_offset` | Bound |
| `H5Pget_fclose_degree` | Bound |
| `H5Pget_file_image` | Bound |
| `H5Pget_file_image_callbacks` | Bound |
| `H5Pget_file_locking` | Bound |
| `H5Pget_file_space` | Bound |
| `H5Pget_file_space_page_size` | Bound |
| `H5Pget_file_space_strategy` | Bound |
| `H5Pget_fill_time` | Bound |
| `H5Pget_fill_value` | Bound |
| `H5Pget_filter_by_id1` | Gap |
| `H5Pget_filter_by_id2` | Bound |
| `H5Pget_filter1` | Gap |
| `H5Pget_filter2` | Bound |
| `H5Pget_gc_references` | Bound |
| `H5Pget_hyper_vector_size` | Bound |
| `H5Pget_istore_k` | Bound |
| `H5Pget_layout` | Bound |
| `H5Pget_libver_bounds` | Bound |
| `H5Pget_link_creation_order` | Bound |
| `H5Pget_link_phase_change` | Bound |
| `H5Pget_local_heap_size_hint` | Bound |
| `H5Pget_map_iterate_hints` | Unavailable in packaged serial build |
| `H5Pget_mcdt_search_cb` | Bound |
| `H5Pget_mdc_config` | Bound |
| `H5Pget_mdc_image_config` | Bound |
| `H5Pget_mdc_log_options` | Bound |
| `H5Pget_meta_block_size` | Bound |
| `H5Pget_metadata_read_attempts` | Bound |
| `H5Pget_modify_write_buf` | Bound |
| `H5Pget_mpi_params` | Unavailable in packaged serial build |
| `H5Pget_mpio_actual_chunk_opt_mode` | Unavailable in packaged serial build |
| `H5Pget_mpio_actual_io_mode` | Unavailable in packaged serial build |
| `H5Pget_mpio_no_collective_cause` | Unavailable in packaged serial build |
| `H5Pget_multi_type` | Bound |
| `H5Pget_nfilters` | Bound |
| `H5Pget_nlinks` | Bound |
| `H5Pget_no_selection_io_cause` | Bound |
| `H5Pget_nprops` | Bound |
| `H5Pget_obj_track_times` | Bound |
| `H5Pget_object_flush_cb` | Bound |
| `H5Pget_page_buffer_size` | Bound |
| `H5Pget_preserve` | Gap |
| `H5Pget_relax_file_integrity_checks` | Bound |
| `H5Pget_selection_io` | Bound |
| `H5Pget_shared_mesg_index` | Bound |
| `H5Pget_shared_mesg_nindexes` | Bound |
| `H5Pget_shared_mesg_phase_change` | Bound |
| `H5Pget_sieve_buf_size` | Bound |
| `H5Pget_size` | Bound |
| `H5Pget_sizes` | Bound |
| `H5Pget_small_data_block_size` | Bound |
| `H5Pget_sym_k` | Bound |
| `H5Pget_type_conv_cb` | Bound |
| `H5Pget_userblock` | Bound |
| `H5Pget_version` | Bound |
| `H5Pget_virtual_count` | Bound |
| `H5Pget_virtual_dsetname` | Bound |
| `H5Pget_virtual_filename` | Bound |
| `H5Pget_virtual_prefix` | Bound |
| `H5Pget_virtual_printf_gap` | Bound |
| `H5Pget_virtual_spatial_tree` | Bound |
| `H5Pget_virtual_srcspace` | Bound |
| `H5Pget_virtual_view` | Bound |
| `H5Pget_virtual_vspace` | Bound |
| `H5Pget_vlen_mem_manager` | Bound |
| `H5Pget_vol_cap_flags` | Bound |
| `H5Pget_vol_id` | Bound |
| `H5Pget_vol_info` | Bound |
| `H5Pinsert1` | Gap |
| `H5Pinsert2` | Bound |
| `H5Pisa_class` | Bound |
| `H5Piterate` | Bound |
| `H5Pmodify_filter` | Bound |
| `H5Pregister1` | Gap |
| `H5Pregister2` | Bound |
| `H5Premove` | Bound |
| `H5Premove_filter` | Bound |
| `H5Pset` | Bound |
| `H5Pset_alignment` | Bound |
| `H5Pset_all_coll_metadata_ops` | Unavailable in packaged serial build |
| `H5Pset_alloc_time` | Bound |
| `H5Pset_append_flush` | Bound |
| `H5Pset_attr_creation_order` | Bound |
| `H5Pset_attr_phase_change` | Bound |
| `H5Pset_btree_ratios` | Bound |
| `H5Pset_buffer` | Bound |
| `H5Pset_cache` | Bound |
| `H5Pset_char_encoding` | Bound |
| `H5Pset_chunk` | Bound |
| `H5Pset_chunk_cache` | Bound |
| `H5Pset_chunk_opts` | Bound |
| `H5Pset_coll_metadata_write` | Unavailable in packaged serial build |
| `H5Pset_copy_object` | Bound |
| `H5Pset_core_write_tracking` | Bound |
| `H5Pset_create_intermediate_group` | Bound |
| `H5Pset_data_transform` | Bound |
| `H5Pset_dataset_io_hyperslab_selection` | Bound |
| `H5Pset_deflate` | Bound |
| `H5Pset_driver` | Bound |
| `H5Pset_driver_by_name` | Bound |
| `H5Pset_driver_by_value` | Bound |
| `H5Pset_dset_no_attrs_hint` | Bound |
| `H5Pset_edc_check` | Bound |
| `H5Pset_efile_prefix` | Bound |
| `H5Pset_elink_acc_flags` | Bound |
| `H5Pset_elink_cb` | Bound |
| `H5Pset_elink_fapl` | Bound |
| `H5Pset_elink_file_cache_size` | Bound |
| `H5Pset_elink_prefix` | Bound |
| `H5Pset_est_link_info` | Bound |
| `H5Pset_evict_on_close` | Bound |
| `H5Pset_external` | Bound |
| `H5Pset_family_offset` | Bound |
| `H5Pset_fclose_degree` | Bound |
| `H5Pset_file_image` | Bound |
| `H5Pset_file_image_callbacks` | Bound |
| `H5Pset_file_locking` | Bound |
| `H5Pset_file_space` | Bound |
| `H5Pset_file_space_page_size` | Bound |
| `H5Pset_file_space_strategy` | Bound |
| `H5Pset_fill_time` | Bound |
| `H5Pset_fill_value` | Bound |
| `H5Pset_filter` | Bound |
| `H5Pset_filter_callback` | Bound |
| `H5Pset_fletcher32` | Bound |
| `H5Pset_gc_references` | Bound |
| `H5Pset_hyper_vector_size` | Bound |
| `H5Pset_istore_k` | Bound |
| `H5Pset_layout` | Bound |
| `H5Pset_libver_bounds` | Bound |
| `H5Pset_link_creation_order` | Bound |
| `H5Pset_link_phase_change` | Bound |
| `H5Pset_local_heap_size_hint` | Bound |
| `H5Pset_map_iterate_hints` | Unavailable in packaged serial build |
| `H5Pset_mcdt_search_cb` | Bound |
| `H5Pset_mdc_config` | Bound |
| `H5Pset_mdc_image_config` | Bound |
| `H5Pset_mdc_log_options` | Bound |
| `H5Pset_meta_block_size` | Bound |
| `H5Pset_metadata_read_attempts` | Bound |
| `H5Pset_modify_write_buf` | Bound |
| `H5Pset_mpi_params` | Unavailable in packaged serial build |
| `H5Pset_multi_type` | Bound |
| `H5Pset_nbit` | Bound |
| `H5Pset_nlinks` | Bound |
| `H5Pset_obj_track_times` | Bound |
| `H5Pset_object_flush_cb` | Bound |
| `H5Pset_page_buffer_size` | Bound |
| `H5Pset_preserve` | Gap |
| `H5Pset_relax_file_integrity_checks` | Bound |
| `H5Pset_scaleoffset` | Bound |
| `H5Pset_selection_io` | Bound |
| `H5Pset_shared_mesg_index` | Bound |
| `H5Pset_shared_mesg_nindexes` | Bound |
| `H5Pset_shared_mesg_phase_change` | Bound |
| `H5Pset_shuffle` | Bound |
| `H5Pset_sieve_buf_size` | Bound |
| `H5Pset_sizes` | Bound |
| `H5Pset_small_data_block_size` | Bound |
| `H5Pset_sym_k` | Bound |
| `H5Pset_szip` | Bound |
| `H5Pset_type_conv_cb` | Bound |
| `H5Pset_userblock` | Bound |
| `H5Pset_virtual` | Bound |
| `H5Pset_virtual_prefix` | Bound |
| `H5Pset_virtual_printf_gap` | Bound |
| `H5Pset_virtual_spatial_tree` | Bound |
| `H5Pset_virtual_view` | Bound |
| `H5Pset_vlen_mem_manager` | Bound |
| `H5Pset_vol` | Bound |
| `H5Punregister` | Bound |

## H5PT

Native header: `H5PTpublic.h`.

| Native function | C# coverage |
|---|---|
| `H5PTappend` | Gap |
| `H5PTclose` | Gap |
| `H5PTcreate` | Gap |
| `H5PTcreate_fl` | Gap |
| `H5PTcreate_index` | Gap |
| `H5PTfree_vlen_buff` | Gap |
| `H5PTget_dataset` | Gap |
| `H5PTget_index` | Gap |
| `H5PTget_next` | Gap |
| `H5PTget_num_packets` | Gap |
| `H5PTget_type` | Gap |
| `H5PTis_valid` | Gap |
| `H5PTis_varlen` | Gap |
| `H5PTopen` | Gap |
| `H5PTread_packets` | Gap |
| `H5PTset_index` | Gap |

## H5

Native header: `H5public.h`.

| Native function | C# coverage |
|---|---|
| `H5allocate_memory` | Bound |
| `H5atclose` | Gap |
| `H5check_version` | Gap |
| `H5close` | Bound |
| `H5dont_atexit` | Bound |
| `H5free_memory` | Bound |
| `H5garbage_collect` | Bound |
| `H5get_free_list_sizes` | Bound |
| `H5get_libversion` | Bound |
| `H5is_library_terminating` | Bound |
| `H5is_library_threadsafe` | Bound |
| `H5open` | Bound |
| `H5resize_memory` | Bound |
| `H5set_free_list_limits` | Bound |

## H5R

Native header: `H5Rpublic.h`.

| Native function | C# coverage |
|---|---|
| `H5Rcopy` | Bound |
| `H5Rcreate` | Bound |
| `H5Rcreate_attr` | Bound |
| `H5Rcreate_object` | Bound |
| `H5Rcreate_region` | Bound |
| `H5Rdereference1` | Gap |
| `H5Rdereference2` | Bound |
| `H5Rdestroy` | Bound |
| `H5Requal` | Bound |
| `H5Rget_attr_name` | Bound |
| `H5Rget_file_name` | Bound |
| `H5Rget_name` | Bound |
| `H5Rget_obj_name` | Bound |
| `H5Rget_obj_type1` | Gap |
| `H5Rget_obj_type2` | Bound |
| `H5Rget_obj_type3` | Bound |
| `H5Rget_region` | Bound |
| `H5Rget_type` | Bound |
| `H5Ropen_attr` | Bound |
| `H5Ropen_attr_async` | Gap |
| `H5Ropen_object` | Bound |
| `H5Ropen_object_async` | Gap |
| `H5Ropen_region` | Bound |
| `H5Ropen_region_async` | Gap |

## H5S

Native header: `H5Spublic.h`.

| Native function | C# coverage |
|---|---|
| `H5Sclose` | Bound |
| `H5Scombine_hyperslab` | Bound |
| `H5Scombine_select` | Bound |
| `H5Scopy` | Bound |
| `H5Screate` | Bound |
| `H5Screate_simple` | Bound |
| `H5Sdecode` | Bound |
| `H5Sencode1` | Bound |
| `H5Sencode2` | Bound |
| `H5Sextent_copy` | Bound |
| `H5Sextent_equal` | Bound |
| `H5Sget_regular_hyperslab` | Bound |
| `H5Sget_select_bounds` | Bound |
| `H5Sget_select_elem_npoints` | Bound |
| `H5Sget_select_elem_pointlist` | Bound |
| `H5Sget_select_hyper_blocklist` | Bound |
| `H5Sget_select_hyper_nblocks` | Bound |
| `H5Sget_select_npoints` | Bound |
| `H5Sget_select_type` | Bound |
| `H5Sget_simple_extent_dims` | Bound |
| `H5Sget_simple_extent_ndims` | Bound |
| `H5Sget_simple_extent_npoints` | Bound |
| `H5Sget_simple_extent_type` | Bound |
| `H5Sis_regular_hyperslab` | Bound |
| `H5Sis_simple` | Bound |
| `H5Smodify_select` | Bound |
| `H5Soffset_simple` | Bound |
| `H5Ssel_iter_close` | Bound |
| `H5Ssel_iter_create` | Bound |
| `H5Ssel_iter_get_seq_list` | Bound |
| `H5Ssel_iter_reset` | Bound |
| `H5Sselect_adjust` | Bound |
| `H5Sselect_all` | Bound |
| `H5Sselect_copy` | Bound |
| `H5Sselect_elements` | Bound |
| `H5Sselect_hyperslab` | Bound |
| `H5Sselect_intersect_block` | Bound |
| `H5Sselect_none` | Bound |
| `H5Sselect_project_intersection` | Bound |
| `H5Sselect_shape_same` | Bound |
| `H5Sselect_valid` | Bound |
| `H5Sset_extent_none` | Bound |
| `H5Sset_extent_simple` | Bound |

## H5TB

Native header: `H5TBpublic.h`.

| Native function | C# coverage |
|---|---|
| `H5TBadd_records_from` | Gap |
| `H5TBAget_fill` | Gap |
| `H5TBAget_title` | Gap |
| `H5TBappend_records` | Gap |
| `H5TBcombine_tables` | Gap |
| `H5TBdelete_field` | Gap |
| `H5TBdelete_record` | Gap |
| `H5TBget_field_info` | Gap |
| `H5TBget_table_info` | Gap |
| `H5TBinsert_field` | Gap |
| `H5TBinsert_record` | Gap |
| `H5TBmake_table` | Gap |
| `H5TBread_fields_index` | Gap |
| `H5TBread_fields_name` | Gap |
| `H5TBread_records` | Gap |
| `H5TBread_table` | Gap |
| `H5TBwrite_fields_index` | Gap |
| `H5TBwrite_fields_name` | Gap |
| `H5TBwrite_records` | Gap |

## H5T

Native header: `H5Tpublic.h`.

| Native function | C# coverage |
|---|---|
| `H5Tarray_create1` | Gap |
| `H5Tarray_create2` | Bound |
| `H5Tclose` | Bound |
| `H5Tclose_async` | Gap |
| `H5Tcommit_anon` | Bound |
| `H5Tcommit_async` | Gap |
| `H5Tcommit1` | Gap |
| `H5Tcommit2` | Bound |
| `H5Tcommitted` | Bound |
| `H5Tcomplex_create` | Bound |
| `H5Tconvert` | Bound |
| `H5Tcopy` | Bound |
| `H5Tcreate` | Bound |
| `H5Tdecode1` | Bound |
| `H5Tdecode2` | Bound |
| `H5Tdetect_class` | Bound |
| `H5Tencode` | Bound |
| `H5Tenum_create` | Bound |
| `H5Tenum_insert` | Bound |
| `H5Tenum_nameof` | Bound |
| `H5Tenum_valueof` | Bound |
| `H5Tequal` | Bound |
| `H5Tflush` | Bound |
| `H5Tget_array_dims1` | Gap |
| `H5Tget_array_dims2` | Bound |
| `H5Tget_array_ndims` | Bound |
| `H5Tget_class` | Bound |
| `H5Tget_create_plist` | Bound |
| `H5Tget_cset` | Bound |
| `H5Tget_ebias` | Bound |
| `H5Tget_fields` | Bound |
| `H5Tget_inpad` | Bound |
| `H5Tget_member_class` | Bound |
| `H5Tget_member_index` | Bound |
| `H5Tget_member_name` | Bound |
| `H5Tget_member_offset` | Bound |
| `H5Tget_member_type` | Bound |
| `H5Tget_member_value` | Bound |
| `H5Tget_native_type` | Bound |
| `H5Tget_nmembers` | Bound |
| `H5Tget_norm` | Bound |
| `H5Tget_offset` | Bound |
| `H5Tget_order` | Bound |
| `H5Tget_pad` | Bound |
| `H5Tget_precision` | Bound |
| `H5Tget_sign` | Bound |
| `H5Tget_size` | Bound |
| `H5Tget_strpad` | Bound |
| `H5Tget_super` | Bound |
| `H5Tget_tag` | Bound |
| `H5Tinsert` | Bound |
| `H5Tis_variable_str` | Bound |
| `H5Tlock` | Bound |
| `H5Topen_async` | Gap |
| `H5Topen1` | Gap |
| `H5Topen2` | Bound |
| `H5Tpack` | Bound |
| `H5Treclaim` | Bound |
| `H5Trefresh` | Bound |
| `H5Tset_cset` | Bound |
| `H5Tset_ebias` | Bound |
| `H5Tset_fields` | Bound |
| `H5Tset_inpad` | Bound |
| `H5Tset_norm` | Bound |
| `H5Tset_offset` | Bound |
| `H5Tset_order` | Bound |
| `H5Tset_pad` | Bound |
| `H5Tset_precision` | Bound |
| `H5Tset_sign` | Bound |
| `H5Tset_size` | Bound |
| `H5Tset_strpad` | Bound |
| `H5Tset_tag` | Bound |
| `H5Tvlen_create` | Bound |

## H5VL

Native header: `H5VLpublic.h`.

| Native function | C# coverage |
|---|---|
| `H5VLclose` | Bound |
| `H5VLget_connector_id` | Bound |
| `H5VLget_connector_id_by_name` | Bound |
| `H5VLget_connector_id_by_value` | Bound |
| `H5VLget_connector_name` | Bound |
| `H5VLis_connector_registered_by_name` | Bound |
| `H5VLis_connector_registered_by_value` | Bound |
| `H5VLobject_is_native` | Bound |
| `H5VLquery_optional` | Bound |
| `H5VLregister_connector_by_name` | Bound |
| `H5VLregister_connector_by_value` | Bound |
| `H5VLunregister_connector` | Bound |

## H5Z

Native header: `H5Zpublic.h`.

| Native function | C# coverage |
|---|---|
| `H5Zfilter_avail` | Bound |
| `H5Zget_filter_info` | Bound |

## H5P predefined identifiers

These identifiers remain owned by HDF5. Optional native datatypes may have negative values when the native compiler lacks support. Map property classes exist even though map operations are not compiled into this package.

| Native global | C# coverage |
|---|---|
| `H5P_CLS_ATTRIBUTE_ACCESS_ID_g` | Bound |
| `H5P_CLS_ATTRIBUTE_CREATE_ID_g` | Bound |
| `H5P_CLS_DATASET_ACCESS_ID_g` | Bound |
| `H5P_CLS_DATASET_CREATE_ID_g` | Bound |
| `H5P_CLS_DATASET_XFER_ID_g` | Bound |
| `H5P_CLS_DATATYPE_ACCESS_ID_g` | Bound |
| `H5P_CLS_DATATYPE_CREATE_ID_g` | Bound |
| `H5P_CLS_FILE_ACCESS_ID_g` | Bound |
| `H5P_CLS_FILE_CREATE_ID_g` | Bound |
| `H5P_CLS_FILE_MOUNT_ID_g` | Bound |
| `H5P_CLS_GROUP_ACCESS_ID_g` | Bound |
| `H5P_CLS_GROUP_CREATE_ID_g` | Bound |
| `H5P_CLS_LINK_ACCESS_ID_g` | Bound |
| `H5P_CLS_LINK_CREATE_ID_g` | Bound |
| `H5P_CLS_MAP_ACCESS_ID_g` | Bound |
| `H5P_CLS_MAP_CREATE_ID_g` | Bound |
| `H5P_CLS_OBJECT_COPY_ID_g` | Bound |
| `H5P_CLS_OBJECT_CREATE_ID_g` | Bound |
| `H5P_CLS_REFERENCE_ACCESS_ID_g` | Bound |
| `H5P_CLS_ROOT_ID_g` | Bound |
| `H5P_CLS_STRING_CREATE_ID_g` | Bound |
| `H5P_CLS_VOL_INITIALIZE_ID_g` | Bound |
| `H5P_LST_ATTRIBUTE_ACCESS_ID_g` | Bound |
| `H5P_LST_ATTRIBUTE_CREATE_ID_g` | Bound |
| `H5P_LST_DATASET_ACCESS_ID_g` | Bound |
| `H5P_LST_DATASET_CREATE_ID_g` | Bound |
| `H5P_LST_DATASET_XFER_ID_g` | Bound |
| `H5P_LST_DATATYPE_ACCESS_ID_g` | Bound |
| `H5P_LST_DATATYPE_CREATE_ID_g` | Bound |
| `H5P_LST_FILE_ACCESS_ID_g` | Bound |
| `H5P_LST_FILE_CREATE_ID_g` | Bound |
| `H5P_LST_FILE_MOUNT_ID_g` | Bound |
| `H5P_LST_GROUP_ACCESS_ID_g` | Bound |
| `H5P_LST_GROUP_CREATE_ID_g` | Bound |
| `H5P_LST_LINK_ACCESS_ID_g` | Bound |
| `H5P_LST_LINK_CREATE_ID_g` | Bound |
| `H5P_LST_MAP_ACCESS_ID_g` | Bound |
| `H5P_LST_MAP_CREATE_ID_g` | Bound |
| `H5P_LST_OBJECT_COPY_ID_g` | Bound |
| `H5P_LST_REFERENCE_ACCESS_ID_g` | Bound |
| `H5P_LST_VOL_INITIALIZE_ID_g` | Bound |

## H5T predefined identifiers

These identifiers remain owned by HDF5. Optional native datatypes may have negative values when the native compiler lacks support. Map property classes exist even though map operations are not compiled into this package.

| Native global | C# coverage |
|---|---|
| `H5T_C_S1_g` | Bound |
| `H5T_COMPLEX_IEEE_F16BE_g` | Bound |
| `H5T_COMPLEX_IEEE_F16LE_g` | Bound |
| `H5T_COMPLEX_IEEE_F32BE_g` | Bound |
| `H5T_COMPLEX_IEEE_F32LE_g` | Bound |
| `H5T_COMPLEX_IEEE_F64BE_g` | Bound |
| `H5T_COMPLEX_IEEE_F64LE_g` | Bound |
| `H5T_FLOAT_BFLOAT16BE_g` | Bound |
| `H5T_FLOAT_BFLOAT16LE_g` | Bound |
| `H5T_FLOAT_F4E2M1_g` | Bound |
| `H5T_FLOAT_F6E2M3_g` | Bound |
| `H5T_FLOAT_F6E3M2_g` | Bound |
| `H5T_FLOAT_F8E4M3_g` | Bound |
| `H5T_FLOAT_F8E5M2_g` | Bound |
| `H5T_FORTRAN_S1_g` | Bound |
| `H5T_IEEE_F16BE_g` | Bound |
| `H5T_IEEE_F16LE_g` | Bound |
| `H5T_IEEE_F32BE_g` | Bound |
| `H5T_IEEE_F32LE_g` | Bound |
| `H5T_IEEE_F64BE_g` | Bound |
| `H5T_IEEE_F64LE_g` | Bound |
| `H5T_NATIVE_B16_g` | Bound |
| `H5T_NATIVE_B32_g` | Bound |
| `H5T_NATIVE_B64_g` | Bound |
| `H5T_NATIVE_B8_g` | Bound |
| `H5T_NATIVE_DOUBLE_COMPLEX_g` | Bound |
| `H5T_NATIVE_DOUBLE_g` | Bound |
| `H5T_NATIVE_FLOAT_COMPLEX_g` | Bound |
| `H5T_NATIVE_FLOAT_g` | Bound |
| `H5T_NATIVE_FLOAT16_g` | Bound |
| `H5T_NATIVE_HADDR_g` | Bound |
| `H5T_NATIVE_HBOOL_g` | Bound |
| `H5T_NATIVE_HERR_g` | Bound |
| `H5T_NATIVE_HSIZE_g` | Bound |
| `H5T_NATIVE_HSSIZE_g` | Bound |
| `H5T_NATIVE_INT_FAST16_g` | Bound |
| `H5T_NATIVE_INT_FAST32_g` | Bound |
| `H5T_NATIVE_INT_FAST64_g` | Bound |
| `H5T_NATIVE_INT_FAST8_g` | Bound |
| `H5T_NATIVE_INT_g` | Bound |
| `H5T_NATIVE_INT_LEAST16_g` | Bound |
| `H5T_NATIVE_INT_LEAST32_g` | Bound |
| `H5T_NATIVE_INT_LEAST64_g` | Bound |
| `H5T_NATIVE_INT_LEAST8_g` | Bound |
| `H5T_NATIVE_INT16_g` | Bound |
| `H5T_NATIVE_INT32_g` | Bound |
| `H5T_NATIVE_INT64_g` | Bound |
| `H5T_NATIVE_INT8_g` | Bound |
| `H5T_NATIVE_LDOUBLE_COMPLEX_g` | Bound |
| `H5T_NATIVE_LDOUBLE_g` | Bound |
| `H5T_NATIVE_LLONG_g` | Bound |
| `H5T_NATIVE_LONG_g` | Bound |
| `H5T_NATIVE_OPAQUE_g` | Bound |
| `H5T_NATIVE_SCHAR_g` | Bound |
| `H5T_NATIVE_SHORT_g` | Bound |
| `H5T_NATIVE_UCHAR_g` | Bound |
| `H5T_NATIVE_UINT_FAST16_g` | Bound |
| `H5T_NATIVE_UINT_FAST32_g` | Bound |
| `H5T_NATIVE_UINT_FAST64_g` | Bound |
| `H5T_NATIVE_UINT_FAST8_g` | Bound |
| `H5T_NATIVE_UINT_g` | Bound |
| `H5T_NATIVE_UINT_LEAST16_g` | Bound |
| `H5T_NATIVE_UINT_LEAST32_g` | Bound |
| `H5T_NATIVE_UINT_LEAST64_g` | Bound |
| `H5T_NATIVE_UINT_LEAST8_g` | Bound |
| `H5T_NATIVE_UINT16_g` | Bound |
| `H5T_NATIVE_UINT32_g` | Bound |
| `H5T_NATIVE_UINT64_g` | Bound |
| `H5T_NATIVE_UINT8_g` | Bound |
| `H5T_NATIVE_ULLONG_g` | Bound |
| `H5T_NATIVE_ULONG_g` | Bound |
| `H5T_NATIVE_USHORT_g` | Bound |
| `H5T_STD_B16BE_g` | Bound |
| `H5T_STD_B16LE_g` | Bound |
| `H5T_STD_B32BE_g` | Bound |
| `H5T_STD_B32LE_g` | Bound |
| `H5T_STD_B64BE_g` | Bound |
| `H5T_STD_B64LE_g` | Bound |
| `H5T_STD_B8BE_g` | Bound |
| `H5T_STD_B8LE_g` | Bound |
| `H5T_STD_I16BE_g` | Bound |
| `H5T_STD_I16LE_g` | Bound |
| `H5T_STD_I32BE_g` | Bound |
| `H5T_STD_I32LE_g` | Bound |
| `H5T_STD_I64BE_g` | Bound |
| `H5T_STD_I64LE_g` | Bound |
| `H5T_STD_I8BE_g` | Bound |
| `H5T_STD_I8LE_g` | Bound |
| `H5T_STD_REF_DSETREG_g` | Bound |
| `H5T_STD_REF_g` | Bound |
| `H5T_STD_REF_OBJ_g` | Bound |
| `H5T_STD_U16BE_g` | Bound |
| `H5T_STD_U16LE_g` | Bound |
| `H5T_STD_U32BE_g` | Bound |
| `H5T_STD_U32LE_g` | Bound |
| `H5T_STD_U64BE_g` | Bound |
| `H5T_STD_U64LE_g` | Bound |
| `H5T_STD_U8BE_g` | Bound |
| `H5T_STD_U8LE_g` | Bound |
| `H5T_UNIX_D32BE_g` | Bound |
| `H5T_UNIX_D32LE_g` | Bound |
| `H5T_UNIX_D64BE_g` | Bound |
| `H5T_UNIX_D64LE_g` | Bound |
| `H5T_VAX_F32_g` | Bound |
| `H5T_VAX_F64_g` | Bound |
