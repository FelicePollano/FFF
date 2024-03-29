﻿using AutoMapper;
using Fff.Crawler;
using FFFui.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFFui
{
    static class TheMapper
    {
        public static Mapper Mapper;
       
        static TheMapper()
        {
            var config = new MapperConfiguration(cfg => {
                                                            cfg.CreateMap<Result, ResultLineModel>();
                cfg.CreateMap<RepoHelper.HistoryEntry, RepoHistoryLineViewModel>();
                                                        });
            TheMapper.Mapper = new Mapper(config);
        }
        
    }
}
