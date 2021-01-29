using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAdvert.Models;

namespace WebAdvert.Api.Services
{
    public class DynamoDBAdvertStorageService : IAdvertStorageService
    {
        private readonly IMapper _mapper;

        public DynamoDBAdvertStorageService(IMapper mapper)
        {
            _mapper = mapper;
        }
        public async Task<string> Add(AdvertModel model)
        {
            var dbModel = _mapper.Map<AdvertDbModel>(model);
            dbModel.CreationDateTime = DateTime.Now;
            dbModel.Id = Guid.NewGuid().ToString();
            dbModel.Status = AdvertStatus.Pending;
            using (var client = new AmazonDynamoDBClient())
            {
                using (var context = new DynamoDBContext(client))
                {
                    await context.SaveAsync(dbModel);
                }
            }
            return dbModel.Id;
        }

        public async Task<bool> CheckHealthAsync()
        {
            using (var client = new AmazonDynamoDBClient())
            {
                var tableDate = await client.DescribeTableAsync("Adverts");
                return tableDate.Table.TableStatus == TableStatus.ACTIVE;
            }
        }

        public async Task Confirm(ConfirmAdvertModel model)
        {
            using (var client = new AmazonDynamoDBClient())
            {
                using (var context = new DynamoDBContext(client))
                {
                    var record = await context.LoadAsync<AdvertDbModel>(model.Id);
                    if (record == null)
                        throw new KeyNotFoundException($"A record with ID = {model.Id} was not found");
                    if (model.Status == AdvertStatus.Active)
                    {
                        record.Status = AdvertStatus.Active;
                        await context.SaveAsync(record);
                    }
                    else
                        await context.DeleteAsync(record);
                }
            }
        }
    }
}
