﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Rentoolo.Model
{
    public class CraftsManDataHelper
    {
        public static TEntity Get<TEntity>(long id) where TEntity : class
        {
            using (var dc = new RentooloEntities())
            {
                var items = (DbSet<TEntity>)dc.GetType().GetProperty(typeof(TEntity).Name).GetValue(dc);
                var item = items.AsEnumerable().FirstOrDefault(x => Convert.ToInt64(typeof(TEntity).GetProperty("Id").GetValue(x)) == id);
                return item;
            }
        }
        public static List<CraftsMan> GetCraftsMans()
        {
            using (var ctx = new RentooloEntities())
            {
                var list = ctx.CraftsMan.OrderByDescending(x => x.Created).ToList();

                return list;
            }
        }

        public static CraftsMan GetCraftsMan(long id)
        {
            using (var dc = new RentooloEntities())
            {
                CraftsMan item = dc.CraftsMan.FirstOrDefault(x => x.Id == id);

                return item;
            }
        }

        public static long AddCraftsMan(CraftsMan item)
        {
            using (var dc = new RentooloEntities())
            {
                var result = dc.CraftsMan.Add(item);

                var res = dc.SaveChanges();

                return result.Id;
            }
        }

        public static List<CraftsMan> GetCraftsManForMainPage(SellFilter filter)
        {
            // bool isEndDate is needed to undestand correctly startEndDate variable - if its true 
            // it means that startEndDate is endDate else startDate

            using (var ctx = new RentooloEntities())
            {
                var result = ctx.CraftsMan.Select(x => x);
                if (filter.Search != null)
                {
                    if (filter.OnlyInName)
                    {
                        //result = result.filterAdverts(filter.Search,true);
                        result = result.Where(x => x.Сraft.Contains(filter.Search));
                    }
                    else
                    {
                        //result = result.filterAdverts(filter.Search, false);
                        result = result.Where(x => x.Сraft.Contains(filter.Search) || x.Description.Contains(filter.Search));
                    }
                }

                if (filter.StartDate != null)
                {
                    //result = result.filterAdverts((DateTime)filter.StartDate, false);
                    result = result.Where(x => x.Created >= filter.StartDate);
                }

                if (filter.EndDate != null)
                {
                    //result = result.filterAdverts((DateTime)filter.EndDate, true);
                    result = result.Where(x => x.Created <= filter.EndDate);
                }

                if (filter.StartPrice != null)
                {
                    //result = result.filterAdverts((double)filter.StartPrice, true);
                    result = result.Where(x => x.Price >= (double)filter.StartPrice);
                }

                if ((filter.EndPrice != null) && (filter.EndPrice >= filter.StartPrice))
                {
                    //result = result.filterAdverts((double)filter.EndPrice, false);
                    result = result.Where(x => x.Price <= (double)filter.EndPrice);
                }

                if (filter.City != null)
                {
                    //result = result.filterAdverts(filter.City);
                    result = result.Where(x => x.Address.Contains(filter.City));
                }

                switch (filter.SortBy)
                {
                    case "date":
                        result = result.OrderBy(x => x.Created);
                        break;
                    case "price":
                        result = result.OrderBy(x => x.Price);
                        break;
                    case "date_desc":
                        result = result.OrderByDescending(x => x.Created);
                        break;
                    case "price_desc":
                        result = result.OrderByDescending(x => x.Price);
                        break;
                    default:
                        result = result.OrderBy(x => x.Created);
                        break;
                }

                return result.ToList();
            }
        }

        public static int GetCraftsManActiveCount(SellFilter filter)
        {
            using (var ctx = new RentooloEntities())
            {
                if (!string.IsNullOrEmpty(filter.Search))
                {
                    int count = ctx.CraftsMan.Where(x => x.Сraft.Contains(filter.Search) || x.Description.Contains(filter.Search)).Count();

                    return count;
                }
                else
                {
                    int count = ctx.CraftsMan.Count();

                    return count;
                }
            }
        }
    }
}
