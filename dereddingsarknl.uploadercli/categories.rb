class Category
  def initialize(name, ftppath, sitepath)
    @name = name
    @ftppath = ftppath
    @sitepath = sitepath
  end

  def name
    return @name
  end
  def ftppath
    return @ftppath
  end
  def sitepath
    return @sitepath
  end
end