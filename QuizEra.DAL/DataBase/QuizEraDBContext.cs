using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuizEra.DAL.DataBase
{
    internal class QuizEraDBContext:DbContext
    {
        public QuizEraDBContext(DbContextOptions<QuizEraDBContext> options) : base(options)
        {
        }
    }
}
